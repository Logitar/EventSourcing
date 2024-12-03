using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// Represents an interface that allows retrieving and storing events in an Entity Framework Core relational event store.
/// </summary>
public class Repository : Infrastructure.Repository
{
  /// <summary>
  /// Gets the event database context.
  /// </summary>
  protected EventContext EventContext { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Repository"/> class.
  /// </summary>
  /// <param name="eventBus">The event bus.</param>
  /// <param name="eventContext">The event database context.</param>
  /// <param name="eventSerializer">The serializer for events.</param>
  public Repository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer) : base(eventBus, eventSerializer)
  {
    EventContext = eventContext;
  }

  /// <summary>
  /// Persists a list of aggregate changes to the event store.
  /// </summary>
  /// <param name="aggregates">The list of aggregates to persist their changes.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  protected override async Task SaveChangesAsync(IEnumerable<IAggregate> aggregates, CancellationToken cancellationToken)
  {
    IEnumerable<EventEntity> events = aggregates.SelectMany(aggregate => EventEntity.FromChanges(aggregate, EventSerializer));

    EventContext.Events.AddRange(events);
    await EventContext.SaveChangesAsync(cancellationToken);
  }
}
