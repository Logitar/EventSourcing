namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Implements a generic store of events.
/// </summary>
public abstract class EventStore : IEventStore // TODO(fpion): unit tests
{
  /// <summary>
  /// The uncommitted changes tracked by the event store.
  /// </summary>
  protected Queue<AppendToStream> Changes { get; } = [];

  /// <summary>
  /// The event buses.
  /// </summary>
  protected IEnumerable<IEventBus> Buses { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventStore"/> class.
  /// </summary>
  /// <param name="buses">The event buses.</param>
  protected EventStore(IEnumerable<IEventBus> buses)
  {
    Buses = buses;
  }

  /// <summary>
  /// Appends the specified events to the specified stream.
  /// </summary>
  /// <param name="streamId">The stream identifier (optional).</param>
  /// <param name="type">The stream type (optional).</param>
  /// <param name="expectation">An expectation of the stream state.</param>
  /// <param name="events">The events to append.</param>
  /// <returns>The stream identifier. A new identifier will be randomly generated if none is provided.</returns>
  /// <exception cref="ArgumentException">The stream identifier was null, empty or only white-space.</exception>
  public virtual StreamId Append(StreamId? streamId, Type? type, StreamExpectation expectation, IEnumerable<IEvent> events)
  {
    if (streamId.HasValue)
    {
      if (string.IsNullOrWhiteSpace(streamId.Value.Value))
      {
        throw new ArgumentException("The stream identifier cannot be null, empty, nor only white-space.", nameof(streamId));
      }
    }
    else
    {
      streamId = StreamId.NewId();
    }

    if (events.Any())
    {
      AppendToStream change = new(streamId.Value, type, expectation, events);
      Changes.Enqueue(change);
    }

    return streamId.Value;
  }

  /// <summary>
  /// Clears the changes tracked by the event store.
  /// </summary>
  public virtual void ClearChanges()
  {
    Changes.Clear();
  }

  /// <summary>
  /// Save the changes to the event store.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public abstract Task SaveChangesAsync(CancellationToken cancellationToken);

  /// <summary>
  /// Publishes the specified event to the event buses.
  /// </summary>
  /// <param name="event">The event to publish.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  protected virtual async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
  {
    foreach (IEventBus bus in Buses)
    {
      await bus.PublishAsync(@event, cancellationToken);
    }
  }
}
