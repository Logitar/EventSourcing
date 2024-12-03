namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents an interface that allows retrieving and storing events in an event store.
/// </summary>
public abstract class Repository : IRepository
{
  /// <summary>
  /// Initializes a new instance of the <see cref="Repository"/> class.
  /// </summary>
  /// <param name="eventBus">The event bus to publish the events to.</param>
  /// <param name="eventSerializer">The serializer for events.</param>
  protected Repository(IEventBus eventBus, IEventSerializer eventSerializer)
  {
    EventBus = eventBus;
    EventSerializer = eventSerializer;
  }

  /// <summary>
  /// Gets the event bus to publish the events to.
  /// </summary>
  protected IEventBus EventBus { get; }
  /// <summary>
  /// Gets the serializer for events.
  /// </summary>
  protected IEventSerializer EventSerializer { get; }

  /// <summary>
  /// Persists an aggregate to the event store.
  /// </summary>
  /// <param name="aggregate">The aggregate to persist.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public virtual async Task SaveAsync(IAggregate aggregate, CancellationToken cancellationToken)
  {
    await SaveAsync([aggregate], cancellationToken);
  }
  /// <summary>
  /// Persists a list of aggregates to the event store.
  /// </summary>
  /// <param name="aggregates">The list of aggregates to persist.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public virtual async Task SaveAsync(IEnumerable<IAggregate> aggregates, CancellationToken cancellationToken)
  {
    await SaveChangesAsync(aggregates, cancellationToken);

    await PublishAndClearChangesAsync(aggregates, cancellationToken);
  }

  /// <summary>
  /// Persists a list of aggregate changes to the event store.
  /// </summary>
  /// <param name="aggregates">The list of aggregates to persist their changes.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  protected abstract Task SaveChangesAsync(IEnumerable<IAggregate> aggregates, CancellationToken cancellationToken);

  /// <summary>
  /// Publishes to the event bus and clear the changes of the specified aggregates.
  /// </summary>
  /// <param name="aggregates">The list of aggregates to publish their events.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  protected virtual async Task PublishAndClearChangesAsync(IEnumerable<IAggregate> aggregates, CancellationToken cancellationToken)
  {
    foreach (IAggregate aggregate in aggregates)
    {
      if (aggregate.HasChanges)
      {
        foreach (IEvent change in aggregate.Changes)
        {
          await EventBus.PublishAsync(change, cancellationToken);
        }

        aggregate.ClearChanges();
      }
    }
  }
}
