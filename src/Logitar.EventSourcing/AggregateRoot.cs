using System.Reflection;

namespace Logitar.EventSourcing;

public abstract class AggregateRoot
{
  protected AggregateRoot(AggregateId? id = null)
  {
    Id = id ?? AggregateId.NewId();
  }

  public AggregateId Id { get; private set; }
  public long Version { get; private set; }

  public AggregateId CreatedById { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public AggregateId? DeletedById { get; private set; }
  public DateTime? DeletedOn { get; private set; }
  public bool IsDeleted => DeletedById.HasValue && DeletedOn.HasValue;

  public AggregateId UpdatedById { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  private readonly List<DomainEvent> _changes = new();
  public bool HasChanges => _changes.Any();
  public IReadOnlyCollection<DomainEvent> Changes => _changes.AsReadOnly();
  public void ClearChanges() => _changes.Clear();

  public void LoadFromChanges(IEnumerable<DomainEvent> changes)
  {
    foreach (DomainEvent change in changes.OrderBy(x => x.Version))
    {
      Dispatch(change);
    }
  }

  protected void ApplyChange(DomainEvent change)
  {
    change.AggregateId = Id;
    change.Version = Version + 1;

    Dispatch(change);

    _changes.Add(change);
  }
  private void Dispatch(DomainEvent change)
  {
    if (change.AggregateId != Id)
    {
      throw new EventAggregateMismatchException(this, change);
    }
    else if (change.Version < Version)
    {
      throw new CannotApplyPastEventException(this, change);
    }

    Type aggregateType = GetType();
    Type eventType = change.GetType();

    MethodInfo method = aggregateType.GetTypeInfo()
      .GetMethod("Apply", BindingFlags.Instance | BindingFlags.NonPublic, new[] { eventType })
      ?? throw new EventNotSupportedException(aggregateType, eventType);

    method.Invoke(this, new[] { change });

    Version = change.Version;

    if (CreatedById == default || CreatedOn == default)
    {
      CreatedById = change.ActorId;
      CreatedOn = change.OccurredOn;
    }

    switch (change.DeleteAction)
    {
      case DeleteAction.Delete:
        DeletedById = change.ActorId;
        DeletedOn = change.OccurredOn;
        break;
      case DeleteAction.Undelete:
        DeletedById = null;
        DeletedOn = null;
        break;
    }

    UpdatedById = change.ActorId;
    UpdatedOn = change.OccurredOn;
  }

  public override bool Equals(object? obj) => obj is AggregateRoot aggregate
    && aggregate.GetType().Equals(GetType())
    && aggregate.Id == Id;
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  public override string ToString() => $"{GetType()} (Id={Id})";
}
