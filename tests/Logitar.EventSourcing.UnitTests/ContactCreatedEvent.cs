namespace Logitar.EventSourcing;

public record ContactCreatedEvent(AggregateId PersonId, ContactType Type, string Value) : DomainEvent;
