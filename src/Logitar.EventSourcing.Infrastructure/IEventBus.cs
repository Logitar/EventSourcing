namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a bus to which events will be published.
/// </summary>
public interface IEventBus
{
  /// <summary>
  /// Publishes the specified event to the event bus.
  /// </summary>
  /// <param name="change">The event to publish.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task PublishAsync(DomainEvent change, CancellationToken cancellationToken = default);
}
