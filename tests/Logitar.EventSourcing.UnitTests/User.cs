
namespace Logitar.EventSourcing;

public class User : AggregateRoot
{
  public string? UniqueName { get; private set; }

  public bool IsDisabled { get; private set; }

  public DateTime? SignedInOn { get; private set; }

  public User() : base()
  {
  }

  public User(StreamId? id) : base(id)
  {
  }

  public User(string uniqueName, ActorId? actorId = null, StreamId? id = null) : this(id)
  {
    Raise(new UserCreated(uniqueName), actorId);
  }
  protected virtual void Handle(UserCreated @event)
  {
    UniqueName = @event.UniqueName;
  }

  public virtual void LoadFromSnapshot(StreamId id, UserSnapshot snapshot, IEnumerable<IEvent>? changes = null)
  {
    UniqueName = snapshot.UniqueName;

    IsDisabled = snapshot.IsDisabled;

    SignedInOn = snapshot.SignedInOn;

    base.LoadFromSnapshot(id, snapshot, changes);
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new UserDeleted(), actorId);
    }
  }

  public void Disable(ActorId? actorId = null)
  {
    if (!IsDisabled)
    {
      Raise(new UserDisabled(Id, Version + 1, actorId: actorId));
    }
  }
  protected virtual void Handle(UserDisabled _)
  {
    IsDisabled = true;
  }

  public void SignIn(ActorId? actorId = null)
  {
    Raise(new UserSignedIn(), actorId);
  }
  protected virtual void Handle(UserSignedIn @event)
  {
    SignedInOn = @event.OccurredOn;
  }
}

public class UserSnapshot : AggregateSnapshot
{
  public string? UniqueName { get; set; }

  public bool IsDisabled { get; set; }

  public DateTime? SignedInOn { get; set; }
}

public record UserCreated(string UniqueName) : DomainEvent;

public record UserDeleted : DomainEvent, IDeleteEvent;

public record UserDisabled : IIdentifiableEvent, IStreamEvent, IVersionedEvent, IActorEvent, ITemporalEvent
{
  public EventId Id { get; }

  public StreamId StreamId { get; }
  public long Version { get; }

  public ActorId? ActorId { get; }
  public DateTime OccurredOn { get; }

  [JsonConstructor]
  public UserDisabled(EventId id, StreamId streamId, long version, ActorId? actorId, DateTime occurredOn)
    : this(streamId, version, id, actorId, occurredOn)
  {
    Id = id;

    StreamId = streamId;
    Version = version;

    ActorId = actorId;
    OccurredOn = occurredOn;
  }

  public UserDisabled(StreamId streamId, long version, EventId? id = null, ActorId? actorId = null, DateTime? occurredOn = null)
  {
    Id = id ?? EventId.NewId();

    StreamId = streamId;
    Version = version;

    ActorId = actorId;
    OccurredOn = occurredOn ?? DateTime.Now;
  }
}

public record UserEnabled : IEvent;

public record UserPasswordCreated(string PasswordHash) : DomainEvent;

public record UserSignedIn : DomainEvent;
