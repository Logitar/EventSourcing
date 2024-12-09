using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

public class KurrentStore : Infrastructure.EventStore
{
  protected EventStoreClient Client { get; }
  protected IEventConverter Converter { get; }

  public KurrentStore(IEnumerable<IEventBus> buses, EventStoreClient client, IEventConverter converter) : base(buses)
  {
    Client = client;
    Converter = converter;
  }

  public override async Task<Stream?> FetchAsync(StreamId streamId, FetchOptions? options, CancellationToken cancellationToken)
  {
    options ??= new FetchOptions();

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
