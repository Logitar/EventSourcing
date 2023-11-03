namespace Logitar.EventSourcing;

public record PersonCreatedEvent(string FullName) : DomainEvent;
