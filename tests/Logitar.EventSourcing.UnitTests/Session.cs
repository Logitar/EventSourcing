namespace Logitar.EventSourcing;

public class Session : AggregateRoot
{
  public StreamId UserId { get; private set; }

  public Session() : base()
  {
  }

  public Session(StreamId? id) : base(id)
  {
  }

  public Session(User user) : base()
  {
    Raise(new SessionCreated(user.Id));
  }
  protected virtual void Handle(SessionCreated @event)
  {
    UserId = @event.UserId;
  }
}

public record SessionCreated(StreamId UserId) : DomainEvent;
