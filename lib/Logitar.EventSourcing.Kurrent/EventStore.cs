using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Represents an event store in which events can be appended or retrieved from EventStoreDB/Kurrent.
/// </summary>
public class EventStore : Infrastructure.EventStore
{
  /// <summary>
  /// Gets the client to EventStoreDB/Kurrent.
  /// </summary>
  protected EventStoreClient Client { get; }
  /// <summary>
  /// Gets the event converter.
  /// </summary>
  protected IEventConverter Converter { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventStore"/> class.
  /// </summary>
  /// <param name="buses">The event buses.</param>
  /// <param name="client">The client to EventStoreDB/Kurrent.</param>
  /// <param name="converter">The event converter.</param>
  public EventStore(IEnumerable<IEventBus> buses, EventStoreClient client, IEventConverter converter) : base(buses)
  {
    Client = client;
    Converter = converter;
  }

  /// <summary>
  /// Fetches many event streams from the store.
  /// </summary>
  /// <param name="options">The fetch options.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved streams, or an empty collection if none was found.</returns>
  public override async Task<IReadOnlyCollection<Stream>> FetchAsync(FetchStreamsOptions? options, CancellationToken cancellationToken)
  {
    Dictionary<StreamId, List<Event>> events = [];
    Dictionary<StreamId, List<Type>> types = [];

    EventStoreClient.ReadAllStreamResult result = Client.ReadAllAsync(Direction.Forwards, Position.Start, cancellationToken: cancellationToken);
    await foreach (ResolvedEvent resolvedEvent in result)
    {
      EventRecord record = resolvedEvent.Event;
      StreamId streamId = new(record.EventStreamId);

      Event @event = Converter.ToEvent(record);
      if (!events.TryGetValue(streamId, out List<Event>? streamEvents))
      {
        streamEvents = [];
        events[streamId] = streamEvents;
      }
      streamEvents.Add(@event);

      Type? type = Converter.GetStreamType(record);
      if (type != null)
      {
        if (!types.TryGetValue(streamId, out List<Type>? streamTypes))
        {
          streamTypes = [];
          types[streamId] = streamTypes;
        }
        streamTypes.Add(type);
      }
    }

    List<Stream> streams = new(capacity: events.Count);
    foreach (KeyValuePair<StreamId, List<Event>> streamEvents in events)
    {
      Type? type = null;
      if (types.TryGetValue(streamEvents.Key, out List<Type>? streamTypes) && streamTypes.Count == 1)
      {
        type = streamTypes.Single();
      }

      Stream stream = new(streamEvents.Key, type, streamEvents.Value);
      streams.Add(stream);
    }

    return streams.AsReadOnly();
  }
  /// <summary>
  /// Fetches an event stream from the store.
  /// </summary>
  /// <param name="streamId">The identifier of the stream.</param>
  /// <param name="options">The fetch options.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved stream, or null if it was not found.</returns>
  public override async Task<Stream?> FetchAsync(StreamId streamId, FetchStreamOptions? options, CancellationToken cancellationToken)
  {
    options ??= new FetchStreamOptions();

    StreamPosition revision = options.FromVersion > 0 ? StreamPosition.FromInt64(options.FromVersion - 1) : StreamPosition.Start;

    long maxCount = long.MaxValue;
    if (options.ToVersion > 0)
    {
      maxCount = options.FromVersion > 0 ? (options.ToVersion - options.FromVersion + 1) : options.ToVersion;
    }

    EventStoreClient.ReadStreamResult result = Client.ReadStreamAsync(Direction.Forwards, streamId.Value, revision, maxCount, cancellationToken: cancellationToken);
    if (await result.ReadState == ReadState.StreamNotFound)
    {
      return null;
    }

    List<Type> streamTypes = [];
    List<Event> events = [];
    await foreach (ResolvedEvent resolvedEvent in result)
    {
      EventRecord record = resolvedEvent.Event;
      Event @event = Converter.ToEvent(record);
      events.Add(@event);

      Type? type = Converter.GetStreamType(record);
      if (type != null)
      {
        streamTypes.Add(type);
      }
    }
    Type? streamType = streamTypes.Count == 1 ? streamTypes.Single() : null;

    return new Stream(streamId, streamType, events);
  }

  /// <summary>
  /// Saves the unsaved changes in the event store.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public override async Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    Queue<IEvent> events = [];

    while (HasChanges)
    {
      AppendToStream stream = DequeueChange();
      string streamName = stream.Id.Value;
      IEnumerable<EventData> eventData = stream.Events.Select(@event => Converter.ToEventData(@event, stream.Type));

      if (stream.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion)
      {
        StreamRevision expectedRevision = StreamRevision.FromInt64(stream.Expectation.Version - stream.Events.Count() - 1);
        await Client.AppendToStreamAsync(streamName, expectedRevision, eventData, cancellationToken: cancellationToken);
      }
      else
      {
        StreamState expectedState = stream.Expectation.Kind switch
        {
          StreamExpectation.StreamExpectationKind.ShouldExist => StreamState.StreamExists,
          StreamExpectation.StreamExpectationKind.ShouldNotExist => StreamState.NoStream,
          _ => StreamState.Any,
        };
        await Client.AppendToStreamAsync(streamName, expectedState, eventData, cancellationToken: cancellationToken);
      }

      foreach (IEvent @event in stream.Events)
      {
        events.Enqueue(@event);
      }
    }

    await PublishAsync(events, cancellationToken);
  }
}
