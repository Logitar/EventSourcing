namespace Logitar.EventSourcing.Infrastructure;

public interface IEventBus
{
  Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);
}
