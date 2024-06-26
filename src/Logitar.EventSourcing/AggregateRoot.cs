﻿namespace Logitar.EventSourcing;

/// <summary>
/// Represents a domain aggregate, which should be a tangible concept in a domain model.
/// </summary>
public abstract class AggregateRoot
{
  /// <summary>
  /// Gets or sets the identifier of the aggregate.
  /// </summary>
  public AggregateId Id { get; private set; }
  /// <summary>
  /// Gets or sets a value indicating whether or not the aggregate is deleted.
  /// </summary>
  public bool IsDeleted { get; private set; }
  /// <summary>
  /// Gets or sets the version of the aggregate.
  /// </summary>
  public long Version { get; private set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who created the aggregate.
  /// </summary>
  public ActorId CreatedBy { get; protected set; }
  /// <summary>
  /// Gets or sets the date and time when the aggregate was created.
  /// </summary>
  public DateTime CreatedOn { get; protected set; }
  /// <summary>
  /// Gets or sets the identifier of the actor who updated the aggregate lastly.
  /// </summary>
  public ActorId UpdatedBy { get; protected set; }
  /// <summary>
  /// Gets or sets the date and time when the aggregate was updated lastly.
  /// </summary>
  public DateTime UpdatedOn { get; protected set; }

  /// <summary>
  /// The uncommitted changes of the aggregate.
  /// </summary>
  private readonly List<DomainEvent> _changes = [];
  /// <summary>
  /// Gets a value indicating whether or not the aggregate has uncommitted changes.
  /// </summary>
  public bool HasChanges => _changes.Count > 0;
  /// <summary>
  /// Gets the uncommitted changes of the aggregate.
  /// </summary>
  public IReadOnlyCollection<DomainEvent> Changes => _changes.AsReadOnly();
  /// <summary>
  /// Clears the uncommitted changes of the aggregate.
  /// </summary>
  public void ClearChanges() => _changes.Clear();

  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
  /// </summary>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <exception cref="ArgumentException">The identifier value is missing.</exception>
  protected AggregateRoot(AggregateId? id = null)
  {
    if (id.HasValue)
    {
      if (string.IsNullOrWhiteSpace(id.Value.Value))
      {
        throw new ArgumentException("The identifier value is required.", nameof(id));
      }

      Id = id.Value;
    }
    else
    {
      Id = AggregateId.NewId();
    }
  }

  /// <summary>
  /// Loads an aggregate from its changes and assign its identifier.
  /// </summary>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="changes">The changes of the aggregate.</param>
  /// <returns>The loaded aggregate.</returns>
  public static T LoadFromChanges<T>(AggregateId id, IEnumerable<DomainEvent> changes) where T : AggregateRoot, new()
  {
    T aggregate = new()
    {
      Id = id
    };

    IOrderedEnumerable<DomainEvent> ordered = changes.OrderBy(e => e.Version);
    foreach (DomainEvent change in ordered)
    {
      aggregate.Handle(change);
    }

    return aggregate;
  }

  /// <summary>
  /// Raises the specified uncommited change to the current aggregate. The change will be associated to this aggregate, then applied to this aggregate before
  /// being added to the list of uncommited changes.
  /// </summary>
  /// <param name="change">The uncommited change.</param>
  protected void Raise(DomainEvent change) => Raise(change, actorId: null);
  /// <summary>
  /// Raises the specified uncommited change to the current aggregate. The change will be associated to this aggregate, then applied to this aggregate before
  /// being added to the list of uncommited changes.
  /// </summary>
  /// <param name="change">The uncommited change.</param>
  /// <param name="actorId">The identifier of the actor who triggered the event.</param>
  protected void Raise(DomainEvent change, ActorId? actorId) => Raise(change, actorId, occurredOn: null);
  /// <summary>
  /// Raises the specified uncommited change to the current aggregate. The change will be associated to this aggregate, then applied to this aggregate before
  /// being added to the list of uncommited changes.
  /// </summary>
  /// <param name="change">The uncommited change.</param>
  /// <param name="occurredOn">The date and time when the event occurred.</param>
  protected void Raise(DomainEvent change, DateTime? occurredOn) => Raise(change, actorId: null, occurredOn);
  /// <summary>
  /// Raises the specified uncommited change to the current aggregate. The change will be associated to this aggregate, then applied to this aggregate before
  /// being added to the list of uncommited changes.
  /// </summary>
  /// <param name="change">The uncommited change.</param>
  /// <param name="actorId">The identifier of the actor who triggered the event.</param>
  /// <param name="occurredOn">The date and time when the event occurred.</param>
  protected void Raise(DomainEvent change, ActorId? actorId, DateTime? occurredOn)
  {
    change.AggregateId = Id;
    change.Version = Version + 1;

    if (actorId.HasValue)
    {
      change.ActorId = actorId.Value;
    }
    if (occurredOn.HasValue)
    {
      change.OccurredOn = occurredOn.Value;
    }

    Handle(change);

    _changes.Add(change);
  }

  /// <summary>
  /// Handles the specified change in the current aggregate, effectively applying the change and its metadata.
  /// </summary>
  /// <param name="change">The change to dispatch.</param>
  /// <exception cref="CannotApplyPastEventException">The change is past the current aggregate's state.</exception>
  /// <exception cref="EventAggregateMismatchException">The change does not belong to the current aggregate.</exception>
  private void Handle(DomainEvent change)
  {
    if (change.AggregateId != Id)
    {
      throw new EventAggregateMismatchException(this, change);
    }

    if (change.Version < Version)
    {
      throw new CannotApplyPastEventException(this, change);
    }

    Dispatch(change);

    if (change.IsDeleted.HasValue)
    {
      IsDeleted = change.IsDeleted.Value;
    }
    Version = change.Version;

    if (Version <= 1)
    {
      CreatedBy = change.ActorId;
      CreatedOn = change.OccurredOn;
    }
    UpdatedBy = change.ActorId;
    UpdatedOn = change.OccurredOn;
  }

  /// <summary>
  /// Dispatches the specified change to be applied through the current aggregate. This method can be overriden to provide a more efficient way of applying
  /// changes, such as using <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns">type pattern matching</see>
  /// instead of reflection.
  /// </summary>
  /// <param name="change">The change to apply.</param>
  protected virtual void Dispatch(DomainEvent change)
  {
    MethodInfo? apply = GetType().GetMethod("Apply", BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, [change.GetType()], modifiers: []);
    apply?.Invoke(this, new[] { change });
  }

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the aggregate.
  /// </summary>
  /// <param name="obj">The object to be compared to.</param>
  /// <returns>True if the object is equal to the aggregate.</returns>
  public override bool Equals(object? obj)
  {
    return obj is AggregateRoot aggregate && aggregate.GetType().Equals(GetType()) && aggregate.Id == Id;
  }
  /// <summary>
  /// Returns the hash code of the current aggregate.
  /// </summary>
  /// <returns>The hash code.</returns>
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  /// <summary>
  /// Returns a string representation of the aggregate.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => $"{GetType()} (Id={Id})";
}
