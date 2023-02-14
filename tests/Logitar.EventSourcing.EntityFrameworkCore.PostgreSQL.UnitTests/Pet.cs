namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

internal record Pet(string Species, string Name, Person? Owner = null);
