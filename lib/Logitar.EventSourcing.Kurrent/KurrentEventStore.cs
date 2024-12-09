using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Implements a store of events using EventStoreDB/Kurrent.
/// </summary>
public class KurrentEventStore : Infrastructure.EventStore // TODO(fpion): unit & integration tests
{
  /// <summary>
  /// Gets the EventStoreDB/Kurrent client.
  /// </summary>
  protected EventStoreClient Client { get; }
  /// <summary>
  /// Gets the event converter.
  /// </summary>
  protected IEventConverter Converter { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="KurrentEventStore"/> class.
  /// </summary>
  /// <param name="buses">The event buses.</param>
  /// <param name="client">The EventStoreDB/Kurrent client.</param>
  /// <param name="converter">The event converter.</param>
  public KurrentEventStore(IEnumerable<IEventBus> buses, EventStoreClient client, IEventConverter converter) : base(buses)
  {
    Client = client;
    Converter = converter;
  }

  /// <summary>
  /// Fetches a stream of events from the event store.
  /// </summary>
  /// <param name="streamId">The stream identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The found stream, or null.</returns>
  public override async Task<Stream?> FetchAsync(StreamId streamId, CancellationToken cancellationToken)
  {
    // TODO(fpion): streamId guard(s)

    /*
     * EventStreamId: string
     * EventId: Uuid
     * EventNumber: StreamPosition
     * EventType: string
     * Data: byte[]
     * Metadata: byte[]
     * Created: DateTime
     * Position: Position
     * ContentType: string
     */
    // TODO(fpion): OccurredBefore, OccurredAfter, FromVersion, ToVersion, AggregateType, IsDeleted := undefined, null, true, false, ActorId

    EventStoreClient.ReadStreamResult result = Client.ReadStreamAsync(
      Direction.Forwards,
      streamId.Value,
      revision: StreamPosition.Start, // TODO(fpion): FromVersion
      maxCount: long.MaxValue, // TODO(fpion): ToVersion - FromVersion + 1
      cancellationToken: cancellationToken);

    if (await result.ReadState == ReadState.StreamNotFound)
    {
      return null;
    }

    List<Event> events = [];
    await foreach (ResolvedEvent resolved in result)
    {
      EventRecord @event = resolved.Event;
      events.Add(Converter.ToEvent(@event));
    }

    Type? type = null; // TODO(fpion): implement
    long version = 0; // TODO(fpion): implement
    ActorId? createdBy = null; // TODO(fpion): implement
    DateTime? createdOn = null; // TODO(fpion): implement
    ActorId? updatedBy = null; // TODO(fpion): implement
    DateTime? updatedOn = null; // TODO(fpion): implement
    bool isDeleted = false; // TODO(fpion): implement

    return new Stream(streamId, type, version, createdBy, createdOn, updatedBy, updatedOn, isDeleted, events);
  }

  /// <summary>
  /// Save the changes to the event store.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public override async Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    Queue<IEvent> events = [];

    while (Changes.Count > 0)
    {
      AppendToStream change = Changes.Dequeue();

      IEnumerable<EventData> eventData = change.Events.Select(@event => Converter.ToEventData(@event, change.Type));
      if (change.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion)
      {
        long revision = change.Expectation.Version - change.Events.Count() - 1;
        await Client.AppendToStreamAsync(change.Id.Value, StreamRevision.FromInt64(revision), eventData, cancellationToken: cancellationToken);
      }
      else
      {
        StreamState state = change.Expectation.Kind switch
        {
          StreamExpectation.StreamExpectationKind.ShouldExist => StreamState.StreamExists,
          StreamExpectation.StreamExpectationKind.ShouldNotExist => StreamState.NoStream,
          _ => StreamState.Any,
        };
        await Client.AppendToStreamAsync(change.Id.Value, state, eventData, cancellationToken: cancellationToken);
      }

      foreach (IEvent @event in change.Events)
      {
        events.Enqueue(@event);
      }
    }

    await PublishAsync(events, cancellationToken);
  }
}
