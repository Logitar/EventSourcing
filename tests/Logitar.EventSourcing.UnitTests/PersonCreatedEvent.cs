namespace Logitar.EventSourcing;

public class PersonCreatedEvent : DomainEvent
{
  public string FullName { get; }

  public PersonCreatedEvent(string fullName)
  {
    FullName = fullName;
  }
}
