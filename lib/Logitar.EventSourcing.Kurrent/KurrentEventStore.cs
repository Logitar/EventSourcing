using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

public class KurrentEventStore : Infrastructure.EventStore
{
  protected EventStoreClient Client { get; }
  protected IEventConverter Converter { get; }

  public KurrentEventStore(IEnumerable<IEventBus> buses, EventStoreClient client, IEventConverter converter) : base(buses)
  {
    Client = client;
    Converter = converter;
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
