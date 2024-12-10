namespace Logitar.EventSourcing;

public abstract class AggregateRoot : IDeletableAggregate, IVersionedAggregate
{
  public StreamId Id { get; protected set; }
  public long Version { get; protected set; }

  public ActorId? CreatedBy { get; protected set; }
  public DateTime CreatedOn { get; protected set; }

  public ActorId? UpdatedBy { get; protected set; }
  public DateTime UpdatedOn { get; protected set; }

  public bool IsDeleted { get; protected set; }

  private readonly List<IEvent> _changes = [];
  public bool HasChanges => _changes.Count > 0;
  public IReadOnlyCollection<IEvent> Changes => _changes.AsReadOnly();
  public void ClearChanges()
  {
    _changes.Clear();
  }

  protected AggregateRoot() : this(id: null)
  {
  }

  protected AggregateRoot(StreamId? id)
  {
    if (id.HasValue)
    {
      if (string.IsNullOrWhiteSpace(id.Value.Value))
      {
        throw new NotImplementedException();
      }

      Id = id.Value;
    }
    else
    {
      Id = StreamId.NewId();
    }
  }

  public virtual void LoadFromChanges(StreamId id, IEnumerable<IEvent> changes)
  {
    Id = id;

    foreach (IEvent change in changes)
    {
      Apply(change);
    }
  }

  protected virtual void Raise(IEvent @event)
  {
    Raise(@event, actorId: null, occurredOn: null);
  }
  protected virtual void Raise(IEvent @event, ActorId? actorId)
  {
    Raise(@event, actorId, occurredOn: null);
  }
  protected virtual void Raise(IEvent @event, DateTime? occurredOn)
  {
    Raise(@event, actorId: null, occurredOn);
  }
  protected virtual void Raise(IEvent @event, ActorId? actorId, DateTime? occurredOn)
  {
    if (@event is DomainEvent domainEvent)
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

    Apply(@event);

    _changes.Add(@event);
  }

  protected virtual void Apply(IEvent @event)
  {
    if (@event is IStreamEvent stream && stream.StreamId != Id)
    {
      throw new NotImplementedException();
    }
    if (@event is IVersionedEvent versioned && versioned.Version != (Version + 1))
    {
      throw new NotImplementedException();
    }

    Version++;

    ActorId? actorId = @event is IActorEvent actor ? actor.ActorId : null;
    DateTime occurredOn = @event is ITemporalEvent temporal ? temporal.OccurredOn : DateTime.Now;

    if (Version <= 1)
    {
      CreatedBy = actorId;
      CreatedOn = occurredOn;
    }

    UpdatedBy = actorId;
    UpdatedOn = occurredOn;

    if (@event is IDeleteControlEvent control && control.IsDeleted.HasValue)
    {
      IsDeleted = control.IsDeleted.Value;
    }

    Dispatch(@event);
  }
  protected virtual void Dispatch(IEvent @event)
  {
    MethodInfo? apply = GetType().GetMethod("Handle", BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, [@event.GetType()], modifiers: []);
    apply?.Invoke(this, [@event]);
  }

  public override bool Equals(object obj) => obj is AggregateRoot aggregate && aggregate.GetType().Equals(GetType()) && aggregate.Id == Id;
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  public override string ToString() => $"{GetType()} (Id={Id})";
}
