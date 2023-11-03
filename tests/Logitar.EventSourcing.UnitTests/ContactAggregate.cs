namespace Logitar.EventSourcing;

public class ContactAggregate : AggregateRoot
{
  public ContactAggregate(AggregateId id) : base(id)
  {
  }

  public ContactAggregate(PersonAggregate person, ContactType type, string value) : base()
  {
    Raise(new ContactCreatedEvent(person.Id, type, value));
  }

  public AggregateId PersonId { get; private set; }

  public ContactType Type { get; private set; }
  public string Value { get; private set; } = string.Empty;

  protected override void Dispatch(DomainEvent change)
  {
    switch (change)
    {
      case ContactCreatedEvent created:
        Apply(created);
        break;
    }
  }

  private void Apply(ContactCreatedEvent e)
  {
    PersonId = e.PersonId;

    Type = e.Type;
    Value = e.Value;
  }
}
