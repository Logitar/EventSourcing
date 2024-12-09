using Logitar.EventSourcing.Infrastructure;
using Marten;

namespace Logitar.EventSourcing.Marten;

public class MartenStore : EventStore
{
  protected IDocumentStore DocumentStore { get; }

  public MartenStore(IEnumerable<IEventBus> buses, IDocumentStore documentStore) : base(buses)
  {
    DocumentStore = documentStore;
  }

  public override async Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    using IDocumentSession session = await DocumentStore.LightweightSerializableSessionAsync(cancellationToken);

    Queue<IEvent> events = [];

    while (HasChanges)
    {
      AppendToStream stream = DequeueChange();
      Guid? streamId = null;
      try
      {
        streamId = stream.Id.ToGuid();
      }
      catch (Exception)
      {
      }

      if (IsNewStream(stream))
      {
        if (streamId.HasValue)
        {
          session.Events.StartStream(stream.Type, streamId.Value, stream.Events);
        }
        else
        {
          session.Events.StartStream(stream.Type, stream.Id.Value, stream.Events);
        }
      }
      else
      {
        if (streamId.HasValue)
        {
          if (stream.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion)
          {
            session.Events.Append(streamId.Value, stream.Expectation.Version, stream.Events);
          }
          else
          {
            session.Events.Append(streamId.Value, stream.Events);
          }
        }
        else
        {
          if (stream.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion)
          {
            session.Events.Append(stream.Id.Value, stream.Expectation.Version, stream.Events);
          }
          else
          {
            session.Events.Append(stream.Id.Value, stream.Events);
          }
        }
      }

      foreach (IEvent @event in stream.Events)
      {
        events.Enqueue(@event);
      }
    }

    await session.SaveChangesAsync(cancellationToken);

    await PublishAsync(events, cancellationToken);
  }

  protected virtual bool IsNewStream(AppendToStream stream) => stream.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldNotExist
    || (stream.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion && stream.Expectation.Version == stream.Events.Count());
}
