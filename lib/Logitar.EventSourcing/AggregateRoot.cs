namespace Logitar.EventSourcing;

/// <summary>
/// Represents a domain aggregate, which should be a tangible concept in a domain model.
/// </summary>
public abstract class AggregateRoot : IDeletableAggregate, IVersionedAggregate
{
  /// <summary>
  /// Gets or sets the identifier of the aggregate.
  /// </summary>
  public StreamId Id { get; protected set; }
  /// <summary>
  /// Gets or sets the version of the aggregate.
  /// </summary>
  public long Version { get; protected set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who created the aggregate.
  /// </summary>
  public ActorId? CreatedBy { get; protected set; }
  /// <summary>
  /// Gets or sets the date and time when the aggregate was created.
  /// </summary>
  public DateTime CreatedOn { get; protected set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who updated the aggregate lastly.
  /// </summary>
  public ActorId? UpdatedBy { get; protected set; }
  /// <summary>
  /// Gets or sets the date and time when the aggregate was updated lastly.
  /// </summary>
  public DateTime UpdatedOn { get; protected set; }

  /// <summary>
  /// Gets or sets a value indicating whether or not the aggregate is deleted.
  /// </summary>
  public bool IsDeleted { get; protected set; }

  /// <summary>
  /// The uncommitted changes of the aggregate.
  /// </summary>
  private readonly List<IEvent> _changes = [];
  /// <summary>
  /// Gets a value indicating whether or not the aggregate has uncommitted changes.
  /// </summary>
  public bool HasChanges => _changes.Count > 0;
  /// <summary>
  /// Gets the uncommitted changes of the aggregate.
  /// </summary>
  public IReadOnlyCollection<IEvent> Changes => _changes.AsReadOnly();
  /// <summary>
  /// Clears the uncommitted changes of the aggregate.
  /// </summary>
  public void ClearChanges()
  {
    _changes.Clear();
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
  /// </summary>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <exception cref="ArgumentException">The identifier value is missing.</exception>
  protected AggregateRoot(StreamId? id = null)
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
      Id = StreamId.NewId();
    }
  }

  /// <summary>
  /// Loads an aggregate from its changes and assign its identifier.
  /// </summary>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="changes">The changes of the aggregate.</param>
  public virtual void LoadFromChanges(StreamId id, IEnumerable<IEvent> changes)
  {
    Id = id;

    foreach (IEvent change in changes)
    {
      Apply(change);
    }
  }

  /// <summary>
  /// Raises the specified uncommited change to the current aggregate. The change will be associated to this aggregate, then applied to this aggregate before being added to the list of uncommited changes.
  /// </summary>
  /// <param name="change">The uncommited change.</param>
  protected virtual void Raise(IEvent change)
  {
    Raise(change, actorId: null, occurredOn: null);
  }
  /// <summary>
  /// Raises the specified uncommited change to the current aggregate. The change will be associated to this aggregate, then applied to this aggregate before being added to the list of uncommited changes.
  /// </summary>
  /// <param name="change">The uncommited change.</param>
  /// <param name="actorId">The identifier of the actor who triggered the event.</param>
  protected virtual void Raise(IEvent change, ActorId? actorId)
  {
    Raise(change, actorId, occurredOn: null);
  }
  /// <summary>
  /// Raises the specified uncommited change to the current aggregate. The change will be associated to this aggregate, then applied to this aggregate before being added to the list of uncommited changes.
  /// </summary>
  /// <param name="change">The uncommited change.</param>
  /// <param name="occurredOn">The date and time when the event occurred.</param>
  protected virtual void Raise(IEvent change, DateTime? occurredOn)
  {
    Raise(change, actorId: null, occurredOn);
  }
  /// <summary>
  /// Raises the specified uncommited change to the current aggregate. The change will be associated to this aggregate, then applied to this aggregate before being added to the list of uncommited changes.
  /// </summary>
  /// <param name="change">The uncommited change.</param>
  /// <param name="actorId">The identifier of the actor who triggered the event.</param>
  /// <param name="occurredOn">The date and time when the event occurred.</param>
  protected virtual void Raise(IEvent change, ActorId? actorId, DateTime? occurredOn)
  {
    if (change is DomainEvent domainEvent)
    {
      domainEvent.StreamId = Id;
      domainEvent.Version = Version + 1;

      if (actorId.HasValue)
      {
        domainEvent.ActorId = actorId.Value;
      }
      if (occurredOn.HasValue)
      {
        domainEvent.OccurredOn = occurredOn.Value;
      }
    }

    Apply(change);

    _changes.Add(change);
  }

  /// <summary>
  /// Applies the specified change in the current aggregate, updating metadata, dispatching it to subclasses and adding it to uncommitted changes.
  /// </summary>
  /// <param name="change">The change to apply.</param>
  /// <exception cref="StreamMismatchException">The change does not belong to the current aggregate.</exception>
  /// <exception cref="UnexpectedEventVersionException">The change version is not subsequent to the aggregate version.</exception>
  protected virtual void Apply(IEvent change)
  {
    if (change is IStreamEvent stream && stream.StreamId != Id)
    {
      throw new StreamMismatchException(this, stream);
    }
    if (change is IVersionedEvent versioned && versioned.Version != (Version + 1))
    {
      throw new UnexpectedEventVersionException(this, versioned);
    }

    Version++;

    ActorId? actorId = change is IActorEvent actor ? actor.ActorId : null;
    DateTime occurredOn = change is ITemporalEvent temporal ? temporal.OccurredOn : DateTime.Now;

    if (Version <= 1)
    {
      CreatedBy = actorId;
      CreatedOn = occurredOn;
    }

    UpdatedBy = actorId;
    UpdatedOn = occurredOn;

    if (change is IDeleteControlEvent control && control.IsDeleted.HasValue)
    {
      IsDeleted = control.IsDeleted.Value;
    }
    else if (change is IDeleteEvent && change is not IUndeleteEvent)
    {
      IsDeleted = true;
    }
    else if (change is IUndeleteEvent && change is not IDeleteEvent)
    {
      IsDeleted = false;
    }

    Dispatch(change);
  }
  /// <summary>
  /// Dispatches the specified change to be applied through the current aggregate. This method can be overriden to provide a more efficient way of applying changes, such as using <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns">type pattern matching</see> instead of reflection.
  /// </summary>
  /// <param name="change">The change to apply.</param>
  protected virtual void Dispatch(IEvent change)
  {
    MethodInfo? apply = GetType().GetMethod("Handle", BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, [change.GetType()], modifiers: []);
    apply?.Invoke(this, [change]);
  }

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the aggregate.
  /// </summary>
  /// <param name="obj">The object to be compared to.</param>
  /// <returns>True if the object is equal to the aggregate.</returns>
  public override bool Equals(object obj) => obj is AggregateRoot aggregate && aggregate.Id == Id;
  /// <summary>
  /// Returns the hash code of the current aggregate.
  /// </summary>
  /// <returns>The hash code.</returns>
  public override int GetHashCode() => Id.GetHashCode();
  /// <summary>
  /// Returns a string representation of the aggregate.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => $"{GetType()} (Id={Id})";
}
