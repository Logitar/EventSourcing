using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// An event store, used to save new events and load aggregates from their events, using EntityFrameworkCore PostgreSQL.
/// </summary>
public class EventStore : IEventStore
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EventStore"/> class using the specified database context and event bus.
  /// </summary>
  /// <param name="context">The database context</param>
  /// <param name="eventBus">The event bus</param>
  public EventStore(EventContext context, IEventBus eventBus)
  {
    Context = context;
    EventBus = eventBus;
  }

  /// <summary>
  /// The database context used to save and retrieve events
  /// </summary>
  protected EventContext Context { get; }
  /// <summary>
  /// The event bus used to publish domain events
  /// </summary>
  protected IEventBus EventBus { get; }

  /// <summary>
  /// Loads an aggregate of the specified type by its aggregate identifier to its most recent version.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="id">The aggregate identifier</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The loaded aggregate or null if none</returns>
  public async Task<T?> LoadAsync<T>(AggregateId id, CancellationToken cancellationToken = default) where T : AggregateRoot
  {
    return await LoadAsync<T>(id, version: null, includeDeleted: false, cancellationToken);
  }
  /// <summary>
  /// Loads an aggregate of the specified type by its aggregate identifier, up to the specified version.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="id">The aggregate identifier</param>
  /// <param name="version">The aggregate version</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The loaded aggregate or null if none</returns>
  public async Task<T?> LoadAsync<T>(AggregateId id, long version, CancellationToken cancellationToken = default) where T : AggregateRoot
  {
    return await LoadAsync<T>(id, version, includeDeleted: false, cancellationToken);
  }
  /// <summary>
  /// Loads an aggregate of the specified type by its aggregate identifier to its most recent version.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="id">The aggregate identifier</param>
  /// <param name="version">The aggregate version</param>
  /// <param name="includeDeleted">A value indicating whether or not the aggregate should be loaded if it is deleted</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The loaded aggregate or null if none</returns>
  public async Task<T?> LoadAsync<T>(AggregateId id, bool includeDeleted, CancellationToken cancellationToken = default) where T : AggregateRoot
  {
    return await LoadAsync<T>(id, version: null, includeDeleted, cancellationToken);
  }
  /// <summary>
  /// Loads tan aggregate of the specified type by its aggregate identifier, up to the specified version.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="id">The aggregate identifier</param>
  /// <param name="version">The aggregate version</param>
  /// <param name="includeDeleted">A value indicating whether or not the aggregate should be loaded if it is deleted</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The loaded aggregate or null if none</returns>
  public async Task<T?> LoadAsync<T>(AggregateId id, long? version, bool includeDeleted, CancellationToken cancellationToken) where T : AggregateRoot
  {
    string aggregateType = typeof(T).GetName();

    IQueryable<EventEntity> query = Context.Events.AsNoTracking()
      .Where(x => x.AggregateType == aggregateType && x.AggregateId == id.Value);
    if (version.HasValue)
    {
      query = query.Where(x => x.Version <= version.Value);
    }
    EventEntity[] events = await query.OrderBy(x => x.Version).ToArrayAsync(cancellationToken);

    return Load<T>(id, events, includeDeleted);
  }

  /// <summary>
  /// Loads a list of aggregates of the specified type by their aggregate identifier.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="ids">The aggregate identifiers</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The loaded aggregates or an empty collection</returns>
  public async Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken) where T : AggregateRoot
  {
    return await LoadAsync<T>(ids, includeDeleted: false, cancellationToken);
  }
  /// <summary>
  /// Loads a list of aggregates of the specified type by their aggregate identifier.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="ids">The aggregate identifiers</param>
  /// <param name="includeDeleted">A value indicating whether or not deleted aggregates should be loaded</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The loaded aggregates or an empty collection</returns>
  public async Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, bool includeDeleted, CancellationToken cancellationToken) where T : AggregateRoot
  {
    string aggregateType = typeof(T).GetName();
    IEnumerable<string> idValues = ids.Select(id => id.Value);

    EventEntity[] events = await Context.Events.AsNoTracking()
      .Where(x => x.AggregateType == aggregateType && idValues.Contains(x.AggregateId))
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<T>(events.GroupBy(e => new AggregateId(e.AggregateId)), includeDeleted);
  }

  /// <summary>
  /// Loads all the aggregates of the specified type.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The loaded aggregates or an empty collection</returns>
  public async Task<IEnumerable<T>> LoadAsync<T>(CancellationToken cancellationToken = default) where T : AggregateRoot
  {
    return await LoadAsync<T>(includeDeleted: false, cancellationToken);
  }
  /// <summary>
  /// Loads all the aggregates of the specified type.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="includeDeleted">A value indicating whether or not the aggregate should be loaded if it is deleted</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The loaded aggregates or an empty collection</returns>
  public async Task<IEnumerable<T>> LoadAsync<T>(bool includeDeleted, CancellationToken cancellationToken = default) where T : AggregateRoot
  {
    string aggregateType = typeof(T).GetName();

    EventEntity[] events = await Context.Events.AsNoTracking()
      .Where(x => x.AggregateType == aggregateType)
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<T>(events.GroupBy(e => new AggregateId(e.AggregateId)), includeDeleted);
  }

  /// <summary>
  /// Loads an aggregate of the specified type by its list of events.
  /// </summary>
  /// <typeparam name="T">The aggregate type.</typeparam>
  /// <param name="events">The list of events.</param>
  /// <param name="includeDeleted">A value indicating whether or not the aggregate should be loaded if it is deleted.</param>
  /// <returns>The loaded aggregate or null if none.</returns>
  protected virtual T? Load<T>(IEnumerable<EventEntity> events, bool includeDeleted = false) where T : AggregateRoot
  {
    if (!events.Any())
    {
      return null;
    }

    return Load<T>(new AggregateId(events.First().AggregateId), events, includeDeleted);
  }
  /// <summary>
  /// Loads an aggregate of the specified type by its aggregate identifier and list of events.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="id">The aggregate identifier</param>
  /// <param name="events">The list of events</param>
  /// <param name="includeDeleted">A value indicating whether or not the aggregate should be loaded if it is deleted</param>
  /// <returns>The loaded aggregate or null if none</returns>
  protected virtual T? Load<T>(AggregateId id, IEnumerable<EventEntity> events, bool includeDeleted = false) where T : AggregateRoot
  {
    if (!events.Any())
    {
      return null;
    }

    ConstructorInfo constructor = GetConstructor<T>();

    return Load<T>(constructor, id, events, includeDeleted);
  }

  /// <summary>
  /// Loads a list of aggregates of the specified type
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="groupedEvents">The domain events grouped by aggregate identifier</param>
  /// <param name="includeDeleted">A value indicating whether or not deleted aggregates should be loaded</param>
  /// <returns>The loaded aggregates or an empty collection</returns>
  protected virtual IEnumerable<T> Load<T>(IEnumerable<IGrouping<AggregateId, EventEntity>> groupedEvents, bool includeDeleted = false) where T : AggregateRoot
  {
    if (!groupedEvents.Any())
    {
      return Enumerable.Empty<T>();
    }

    ConstructorInfo constructor = GetConstructor<T>();

    List<T> aggregates = new(capacity: groupedEvents.Count());
    foreach (IGrouping<AggregateId, EventEntity> events in groupedEvents)
    {
      aggregates.AddIfNotNull(Load<T>(constructor, events.Key, events, includeDeleted));
    }

    return aggregates.AsReadOnly();
  }

  /// <summary>
  /// Gets the required constructor for the specified aggregate type.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <returns>The constructor</returns>
  /// <exception cref="NotSupportedException">The aggregate type does not define a public constructor with a single <see cref="AggregateId"/> argument.</exception>
  private static ConstructorInfo GetConstructor<T>()
  {
    ConstructorInfo? constructor = typeof(T).GetTypeInfo().GetConstructor(new[] { typeof(AggregateId) });
    if (constructor == null)
    {
      StringBuilder message = new();
      message.AppendLine("The specified aggregate type does not declare a public constructor receiving an AggregateId as its only argument.");
      message.AppendLine($"AggregateType: {typeof(T).GetName()}");
      throw new NotSupportedException(message.ToString());
    }

    return constructor;
  }

  /// <summary>
  /// Loads an aggregate of the specified type using the specified constructor, identifier and list of events.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="constructor">The constructor to use</param>
  /// <param name="id">The aggregate identifier</param>
  /// <param name="events">The list of events</param>
  /// <param name="includeDeleted">A value indicating whether or not the aggregate should be loaded if it is deleted</param>
  /// <returns>The loaded aggregate or null if none</returns>
  /// <exception cref="InvalidOperationException">The aggregate was null once the constructor was invoked</exception>
  private static T? Load<T>(ConstructorInfo constructor, AggregateId id, IEnumerable<EventEntity> events, bool includeDeleted) where T : AggregateRoot
  {
    T? aggregate = (T?)constructor.Invoke(new object[] { id });
    if (aggregate == null)
    {
      StringBuilder message = new();
      message.AppendLine("The specified aggregate could not be constructed.");
      message.AppendLine($"AggregateType: {typeof(T).GetName()}");
      message.AppendLine($"AggregateId: {id}");
      throw new InvalidOperationException(message.ToString());
    }

    IEnumerable<DomainEvent> changes = events.Select(EventSerializer.Instance.Deserialize);
    aggregate.LoadFromChanges(changes);

    return aggregate.IsDeleted && !includeDeleted ? null : aggregate;
  }

  /// <summary>
  /// Saves the specified aggregate in the event store.
  /// </summary>
  /// <param name="aggregate">The aggregate to save</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The asynchronous operation</returns>
  public async Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
  {
    if (aggregate.HasChanges)
    {
      IEnumerable<EventEntity> events = EventEntity.FromChanges(aggregate);

      Context.Events.AddRange(events);
      await Context.SaveChangesAsync(cancellationToken);

      await PublishAndClearChangesAsync(aggregate, cancellationToken);
    }
  }

  /// <summary>
  /// Saves the specified aggregates in the event store.
  /// </summary>
  /// <param name="aggregates">The aggregates to save</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The asynchronous operation</returns>
  public async Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken)
  {
    IEnumerable<EventEntity> events = aggregates.SelectMany(EventEntity.FromChanges);
    if (events.Any())
    {
      Context.Events.AddRange(events);
      await Context.SaveChangesAsync(cancellationToken);

      foreach (AggregateRoot aggregate in aggregates)
      {
        await PublishAndClearChangesAsync(aggregate, cancellationToken);
      }
    }
  }

  private async Task PublishAndClearChangesAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
  {
    if (aggregate.HasChanges)
    {
      foreach (DomainEvent change in aggregate.Changes)
      {
        await EventBus.PublishAsync(change, cancellationToken);
      }

      aggregate.ClearChanges();
    }
  }
}
