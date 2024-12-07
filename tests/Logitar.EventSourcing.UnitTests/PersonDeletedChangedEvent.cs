namespace Logitar.EventSourcing;

public class PersonDeletedChangedEvent : DomainEvent
{
  public PersonDeletedChangedEvent(bool? isDeleted)
  {
    IsDeleted = isDeleted;
  }
}
