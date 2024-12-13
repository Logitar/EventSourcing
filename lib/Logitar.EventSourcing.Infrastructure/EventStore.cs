namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents an abstraction of an event store in which events can be appended or retrieved.
/// </summary>
public abstract class EventStore : IEventStore
{
  /// <summary>
  /// Gets the event buses.
  /// </summary>
  protected IEnumerable<IEventBus> Buses { get; }

  /// <summary>
  /// Gets the uncommitted changes tracked by the store.
  /// </summary>
  protected Queue<AppendToStream> Changes { get; } = [];
  /// <summary>
  /// Gets a value indicating whether or not the store is tracking uncommitted changes.
  /// </summary>
  public bool HasChanges => Changes.Count > 0;

  /// <summary>
  /// Initializes a new instance of the <see cref="EventStore"/> class.
  /// </summary>
  /// <param name="buses">The event buses.</param>
  protected EventStore(IEnumerable<IEventBus> buses)
  {
    Buses = buses;
  }

  /// <summary>
  /// Appends the specified events into the event store.
  /// </summary>
  /// <param name="streamId">The identifier of the stream. If left null, a new stream identifier will be randomly generated.</param>
  /// <param name="streamType">The type of the stream.</param>
  /// <param name="streamExpectation">An expectation of the stream state.</param>
  /// <param name="events">The events to append.</param>
  /// <returns>The identifier of the stream.</returns>
  /// <exception cref="ArgumentException">The stream identifier was empty.</exception>
  public virtual StreamId Append(StreamId? streamId, Type? streamType, StreamExpectation streamExpectation, IEnumerable<IEvent> events)
  {
    if (streamId.HasValue)
    {
      if (string.IsNullOrWhiteSpace(streamId.Value.Value))
      {
        throw new ArgumentException("The stream identifier is required.", nameof(streamId));
      }
    }
    else
    {
      streamId = StreamId.NewId();
    }

    if (events.Any())
    {
      AppendToStream change = new(streamId.Value, streamType, streamExpectation, events);
      Changes.Enqueue(change);
    }

    return streamId.Value;
  }

  /// <summary>
  /// Clears the unsaved changes in the event store.
  /// </summary>
  public virtual void ClearChanges()
  {
    Changes.Clear();
  }


  /// <summary>
  /// Fetches many event streams from the store.
  /// </summary>
  /// <param name="options">The fetch options.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved streams, or an empty collection if none was found.</returns>
  public abstract Task<IReadOnlyCollection<Stream>> FetchAsync(FetchManyOptions? options, CancellationToken cancellationToken);
  /// <summary>
  /// Fetches an event stream from the store.
  /// </summary>
  /// <param name="streamId">The identifier of the stream.</param>
  /// <param name="options">The fetch options.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved stream, or null if it was not found.</returns>
  public abstract Task<Stream?> FetchAsync(StreamId streamId, FetchOptions? options, CancellationToken cancellationToken);

  /// <summary>
  /// Saves the unsaved changes in the event store.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public abstract Task SaveChangesAsync(CancellationToken cancellationToken);

  /// <summary>
  /// Dequeues the next change from the tracked uncommitted change queue.
  /// </summary>
  /// <returns>The next change.</returns>
  protected virtual AppendToStream DequeueChange() => Changes.Dequeue();

  /// <summary>
  /// Publishes the specified events to all event buses.
  /// </summary>
  /// <param name="events">The events to publish.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  protected virtual async Task PublishAsync(Queue<IEvent> events, CancellationToken cancellationToken)
  {
    while (events.Count > 0)
    {
      IEvent @event = events.Dequeue();
      foreach (IEventBus bus in Buses)
      {
        await bus.PublishAsync(@event, cancellationToken);
      }
    }
  }
}
