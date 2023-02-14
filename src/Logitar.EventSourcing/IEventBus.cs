namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event bus, where events will be published.
/// </summary>
public interface IEventBus
{
  /// <summary>
  /// Publishes the specified event.
  /// </summary>
  /// <param name="change">The event to publish</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The asynchronous operation</returns>
  Task PublishAsync(DomainEvent change, CancellationToken cancellationToken = default);
}
