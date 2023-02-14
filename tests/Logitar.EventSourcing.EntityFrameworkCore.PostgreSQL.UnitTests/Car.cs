namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

internal record Car(int Year, string Make, string Model, Person? Owner = null);
