namespace Logitar.EventSourcing;

public class ContactCreatedEvent : DomainEvent
{
  public AggregateId PersonId { get; }
  public ContactType Type { get; }
  public string Value { get; }

  public ContactCreatedEvent(AggregateId personId, ContactType contactType, string value)
  {
    PersonId = personId;
    Type = contactType;
    Value = value;
  }
}
