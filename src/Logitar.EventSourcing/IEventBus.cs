namespace Logitar.EventSourcing;

public interface IEventBus
{
  Task PublishAsync(DomainEvent change, CancellationToken cancellationToken = default);
}
