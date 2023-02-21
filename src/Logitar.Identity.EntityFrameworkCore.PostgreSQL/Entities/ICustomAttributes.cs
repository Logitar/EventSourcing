namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// Defines shared properties of objects with custom attributes.
/// </summary>
public interface ICustomAttributes
{
  /// <summary>
  /// Gets the aggregate identifier.
  /// </summary>
  string AggregateId { get; }

  /// <summary>
  /// Gets the custom attributes of the object.
  /// </summary>
  string? CustomAttributes { get; }
}
