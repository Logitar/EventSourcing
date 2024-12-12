
namespace Logitar.EventSourcing.Infrastructure;

internal class FakeEventStore : EventStore
{
  public FakeEventStore(IEnumerable<IEventBus> buses) : base(buses)
  {
  }

  public override Task<IReadOnlyCollection<Stream>> FetchAsync(FetchStreamsOptions? options, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public override Task<Stream?> FetchAsync(StreamId streamId, FetchOptions? options, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public override Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public new AppendToStream DequeueChange() => base.DequeueChange();

  public AppendToStream PeekChange() => Changes.Peek();

  public new Task PublishAsync(Queue<IEvent> events, CancellationToken cancellationToken) => base.PublishAsync(events, cancellationToken);
}
