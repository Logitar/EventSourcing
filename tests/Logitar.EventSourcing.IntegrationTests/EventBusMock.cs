using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing;

public class EventBusMock : IEventBus
{
  private readonly Queue<IEvent> _events = [];

  public Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
  {
    _events.Enqueue(@event);
    return Task.CompletedTask;
  }

  public void VerifyPublished(params IEvent[] events)
  {
    foreach (IEvent @event in events)
    {
      IEvent enqueued = _events.Dequeue();
      Assert.Same(@event, enqueued);
    }

    Assert.Empty(_events);
  }
}
