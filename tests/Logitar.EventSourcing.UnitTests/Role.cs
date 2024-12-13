namespace Logitar.EventSourcing;

public class Role : AggregateRoot
{
  public string? UniqueName { get; private set; }

  public Role(string uniqueName, ActorId? actorId = null, StreamId? id = null) : base(id)
  {
    Raise(new RoleCreated(uniqueName), actorId);
  }
  protected virtual void Handle(RoleCreated @event)
  {
    UniqueName = @event.UniqueName;
  }
}

public record RoleCreated(string UniqueName) : DomainEvent;
