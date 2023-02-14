using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

public class EventStore : IEventStore
{
  public EventStore(EventContext context, IEventBus eventBus)
  {
    Context = context;
    EventBus = eventBus;
  }

  protected EventContext Context { get; }
  protected IEventBus EventBus { get; }

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

  protected virtual T? Load<T>(AggregateId id, IEnumerable<EventEntity> events, bool includeDeleted = false) where T : AggregateRoot
  {
    if (!events.Any())
    {
      return null;
    }

    ConstructorInfo constructor = GetConstructor<T>();

    return Load<T>(constructor, id, events, includeDeleted);
  }
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
