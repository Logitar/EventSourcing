namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

internal record CarRegistered(Car Car) : DomainEvent;
