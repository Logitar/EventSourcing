using MediatR;

namespace Logitar.EventSourcing.Demo;

internal class EventBus : IEventBus
{
  private readonly IPublisher _publisher;

  public EventBus(IPublisher publisher)
  {
    _publisher = publisher;
  }

  public async Task PublishAsync(DomainEvent change, CancellationToken cancellationToken = default)
  {
    await _publisher.Publish(change, cancellationToken);
  }
}
