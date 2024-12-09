using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Implements a store of events using EventStoreDB/Kurrent.
/// </summary>
public class KurrentEventStore : Infrastructure.EventStore // TODO(fpion): unit & integration tests
{
  /// <summary>
  /// The EventStoreDB/Kurrent client.
  /// </summary>
  protected EventStoreClient Client { get; }
  /// <summary>
  /// The event converter.
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

    while (events.Count > 0)
    {
      IEvent @event = events.Dequeue();
      await PublishAsync(@event, cancellationToken);
    }
  }
}
