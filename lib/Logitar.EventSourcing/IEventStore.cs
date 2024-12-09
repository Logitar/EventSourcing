namespace Logitar.EventSourcing;

/// <summary>
/// Defines a store of events. Events are organized in streams.
/// See the <see href="https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing">Event Sourcing</see> pattern for more information.
/// </summary>
public interface IEventStore
{
  /// <summary>
  /// Appends the specified events to the specified stream.
  /// </summary>
  /// <param name="streamId">The stream identifier (optional).</param>
  /// <param name="type">The stream type (optional).</param>
  /// <param name="expectation">An expectation of the stream state.</param>
  /// <param name="events">The events to append.</param>
  /// <returns>The stream identifier. A new identifier will be randomly generated if none is provided.</returns>
  StreamId Append(StreamId? streamId, Type? type, StreamExpectation expectation, IEnumerable<IEvent> events);

  /// <summary>
  /// Clears the changes tracked by the event store.
  /// </summary>
  void ClearChanges();

  /// <summary>
  /// Fetches a stream of events from the event store.
  /// </summary>
  /// <param name="streamId">The stream identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The found stream, or null.</returns>
  Task<Stream?> FetchAsync(StreamId streamId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Save the changes to the event store.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
