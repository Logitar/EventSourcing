namespace Logitar.EventSourcing.Infrastructure;

public abstract class EventStore : IEventStore
{
  protected IEnumerable<IEventBus> Buses { get; }

  protected Queue<AppendToStream> Changes { get; } = [];
  public bool HasChanges => Changes.Count > 0;

  protected EventStore(IEnumerable<IEventBus> buses)
  {
    Buses = buses;
  }

  public virtual StreamId Append(StreamId? streamId, Type? streamType, StreamExpectation streamExpectation, IEnumerable<IEvent> events)
  {
    if (streamId.HasValue)
    {
      if (string.IsNullOrWhiteSpace(streamId.Value.Value))
      {
        throw new NotImplementedException();
      }
    }
    else
    {
      streamId = StreamId.NewId();
    }

    AppendToStream change = new(streamId.Value, streamType, streamExpectation, events);
    Changes.Enqueue(change);

    return streamId.Value;
  }

  public virtual void ClearChanges()
  {
    Changes.Clear();
  }

  public abstract Task<Stream?> FetchAsync(StreamId streamId, FetchOptions? options, CancellationToken cancellationToken);

  public abstract Task SaveChangesAsync(CancellationToken cancellationToken);

  protected virtual AppendToStream DequeueChange() => Changes.Dequeue();

  protected virtual async Task PublishAsync(Queue<IEvent> events, CancellationToken cancellationToken)
  {
    while (events.Count > 0)
    {
      IEvent @event = events.Dequeue();
      foreach (IEventBus bus in Buses)
      {
        await bus.PublishAsync(@event, cancellationToken);
      }
    }
  }
}
