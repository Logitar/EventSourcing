namespace Logitar.EventSourcing;

/// <summary>
/// Represents a handler for a specific event.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public interface IEventHandler<TEvent> where TEvent : IEvent
{
  /// <summary>
  /// Handles the specified event.
  /// </summary>
  /// <param name="event">The event to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
