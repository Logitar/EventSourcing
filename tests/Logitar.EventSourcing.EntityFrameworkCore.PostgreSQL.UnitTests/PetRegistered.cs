namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

internal record PetRegistered(Pet Pet) : DomainEvent;
