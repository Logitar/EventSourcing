namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event store in which events can be appended or retrieved.
/// </summary>
public interface IEventStore
{
  /// <summary>
  /// Appends the specified events into the event store.
  /// </summary>
  /// <param name="streamId">The identifier of the stream. If left null, a new stream identifier will be randomly generated.</param>
  /// <param name="streamType">The type of the stream.</param>
  /// <param name="streamExpectation">An expectation of the stream state.</param>
  /// <param name="events">The events to append.</param>
  /// <returns>The identifier of the stream.</returns>
  StreamId Append(StreamId? streamId, Type? streamType, StreamExpectation streamExpectation, IEnumerable<IEvent> events);

  /// <summary>
  /// Gets a value indicating whether or not there are unsaved changes in the event store.
  /// </summary>
  bool HasChanges { get; }
  /// <summary>
  /// Clears the unsaved changes in the event store.
  /// </summary>
  void ClearChanges();

  /// <summary>
  /// Fetches many event streams from the store.
  /// </summary>
  /// <param name="options">The fetch options.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved streams, or an empty collection if none was found.</returns>
  Task<IReadOnlyCollection<Stream>> FetchAsync(FetchStreamsOptions? options = null, CancellationToken cancellationToken = default);
  /// <summary>
  /// Fetches an event stream from the store.
  /// </summary>
  /// <param name="streamId">The identifier of the stream.</param>
  /// <param name="options">The fetch options.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved stream, or null if it was not found.</returns>
  Task<Stream?> FetchAsync(StreamId streamId, FetchOptions? options = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Saves the unsaved changes in the event store.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
