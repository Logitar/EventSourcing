namespace Logitar.EventSourcing.Infrastructure;

internal record UserGenderChanged(Gender? Gender) : DomainEvent;
