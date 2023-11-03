namespace Logitar.EventSourcing;

public class PersonAggregate : AggregateRoot
{
  public PersonAggregate(AggregateId id) : base(id)
  {
  }

  public PersonAggregate(string fullName, ActorId? actorId = null, DateTime? occurredOn = null) : base()
  {
    PersonCreatedEvent created = new(fullName);
    if (actorId.HasValue)
    {
      created.ActorId = actorId.Value;
    }
    if (occurredOn.HasValue)
    {
      created.OccurredOn = occurredOn.Value;
    }

    Raise(created);
  }
  protected virtual void Apply(PersonCreatedEvent e) => FullName = e.FullName;

  public string FullName { get; private set; } = string.Empty;

  public void Delete() => Raise(new PersonDeletedChangedEvent(isDeleted: true));
  public void Undelete() => Raise(new PersonDeletedChangedEvent(isDeleted: false));

  public void Handle(DomainEvent e)
  {
    MethodInfo handle = typeof(AggregateRoot)
      .GetMethod("Handle", BindingFlags.Instance | BindingFlags.NonPublic, new[] { typeof(DomainEvent) })
      ?? throw new InvalidOperationException("The Handle method could not be found.");

    try
    {
      handle.Invoke(this, new[] { e });
    }
    catch (Exception exception)
    {
      if (exception.InnerException == null)
      {
        throw;
      }

      throw exception.InnerException;
    }
  }
}
