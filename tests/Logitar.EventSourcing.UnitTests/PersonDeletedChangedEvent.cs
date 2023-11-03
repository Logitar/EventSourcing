namespace Logitar.EventSourcing;

public record PersonDeletedChangedEvent : DomainEvent
{
  public PersonDeletedChangedEvent(bool? isDeleted)
  {
    IsDeleted = isDeleted;
  }
}
