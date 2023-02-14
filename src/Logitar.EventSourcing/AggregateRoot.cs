using System.Reflection;

namespace Logitar.EventSourcing;

/// <summary>
/// Represents a domain aggregate, typically an object representing an important concept with its boundaries.
/// </summary>
public abstract class AggregateRoot
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateRoot"/> class using the specified aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier; if null, an aggregate identifier will be generated using a random Guid</param>
  protected AggregateRoot(AggregateId? id = null)
  {
    Id = id ?? AggregateId.NewId();
  }

  /// <summary>
  /// The aggregate identifier
  /// </summary>
  public AggregateId Id { get; private set; }
  /// <summary>
  /// The aggregate current version
  /// </summary>
  public long Version { get; private set; }

  /// <summary>
  /// The identifier of the actor who created the aggregate
  /// </summary>
  public AggregateId CreatedById { get; private set; }
  /// <summary>
  /// The date and time when the aggregate was created
  /// </summary>
  public DateTime CreatedOn { get; private set; }

  /// <summary>
  /// The identifier of the actor who deleted the aggregate
  /// </summary>
  public AggregateId? DeletedById { get; private set; }
  /// <summary>
  /// The date and time when the aggregate was deleted
  /// </summary>
  public DateTime? DeletedOn { get; private set; }
  /// <summary>
  /// Returns a value indicating whether or not the aggregate is in a deleted state.
  /// </summary>
  public bool IsDeleted => DeletedById.HasValue && DeletedOn.HasValue;

  /// <summary>
  /// The identifier of the actor who updated the aggregate lastly
  /// </summary>
  public AggregateId UpdatedById { get; private set; }
  /// <summary>
  /// The date and time when the aggregate was updated
  /// </summary>
  public DateTime UpdatedOn { get; private set; }

  /// <summary>
  /// The uncomitted changes
  /// </summary>
  private readonly List<DomainEvent> _changes = new();
  /// <summary>
  /// Returns a value indicating whether or not the aggregate has uncommited changes.
  /// </summary>
  public bool HasChanges => _changes.Any();
  /// <summary>
  /// The uncommited changes
  /// </summary>
  public IReadOnlyCollection<DomainEvent> Changes => _changes.AsReadOnly();
  /// <summary>
  /// Clears the uncommited changes.
  /// </summary>
  public void ClearChanges() => _changes.Clear();

  /// <summary>
  /// Applies the specified change history to the current aggregate. To improve performance,
  /// consider sorting the changes before calling this method if your event store supports it.
  /// </summary>
  /// <param name="changes">The changes to apply</param>
  public void LoadFromChanges(IEnumerable<DomainEvent> changes)
  {
    foreach (DomainEvent change in changes.OrderBy(x => x.Version))
    {
      Dispatch(change);
    }
  }

  /// <summary>
  /// Applies the specified change to the current aggregate.
  /// </summary>
  /// <param name="change">The change to apply</param>
  protected void ApplyChange(DomainEvent change)
  {
    change.AggregateId = Id;
    change.Version = Version + 1;

    Dispatch(change);

    _changes.Add(change);
  }
  /// <summary>
  /// Dispatchs the specified event in the current aggregate, effectively applying the changes and updating the aggregate metadata.
  /// </summary>
  /// <param name="change">The event to apply</param>
  /// <exception cref="EventAggregateMismatchException">The event does not belong to the current aggregate.</exception>
  /// <exception cref="CannotApplyPastEventException">The event is past the current aggregate's state.</exception>
  /// <exception cref="EventNotSupportedException">The current aggregate does not have an Apply method for the event.</exception>
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

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the current aggregate
  /// </summary>
  /// <param name="obj">The object to compare</param>
  /// <returns>True if the object is equal to the current aggregate</returns>
  public override bool Equals(object? obj) => obj is AggregateRoot aggregate
    && aggregate.GetType().Equals(GetType())
    && aggregate.Id == Id;
  /// <summary>
  /// Returns an integer corresponding to the hash code of the current aggregate.
  /// </summary>
  /// <returns>The hash code derived from the aggregate type and identifier</returns>
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  /// <summary>
  /// Returns a string representation of the current aggregate.
  /// </summary>
  /// <returns>The formatted aggregate type and identifier</returns>
  public override string ToString() => $"{GetType()} (Id={Id})";
}
