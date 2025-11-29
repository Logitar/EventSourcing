namespace Logitar.EventSourcing.Infrastructure;

public record UserGenderChanged(Gender? Gender) : DomainEvent;
