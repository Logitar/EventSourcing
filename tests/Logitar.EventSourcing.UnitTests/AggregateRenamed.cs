namespace Logitar.EventSourcing;

public record AggregateRenamed(string Name) : DomainEvent;
