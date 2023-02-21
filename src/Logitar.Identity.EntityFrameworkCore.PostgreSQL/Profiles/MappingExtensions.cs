using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using System.Text.Json;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// Defines static methods to help mapping.
/// </summary>
public static class MappingHelper
{
  /// <summary>
  /// Retrieves the custom attributes of the specified entity.
  /// </summary>
  /// <param name="entity">The source entity.</param>
  /// <param name="destination">The destination object.</param>
  /// <returns>The list of custom attributes.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public static IEnumerable<CustomAttribute> GetCustomAttributes(ICustomAttributes entity, object _)
  {
    if (entity.CustomAttributes == null)
    {
      return Enumerable.Empty<CustomAttribute>();
    }

    Dictionary<string, string> dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.CustomAttributes)
      ?? throw new InvalidOperationException($"The custom attributes could not be deserialized on aggregate 'AggregateId={entity.AggregateId}'.");

    return dictionary.Select(customAttribute => new CustomAttribute
    {
      Key = customAttribute.Key,
      Value = customAttribute.Value
    });
  }

  /// <summary>
  /// Converts the aggregate identifier to a Guid.
  /// </summary>
  /// <param name="entity">The source aggregate.</param>
  /// <param name="destination">The destination object.</param>
  /// <returns>The Guid identifier.</returns>
  public static Guid ToGuid(AggregateEntity entity, object _)
  {
    AggregateId id = new(entity.AggregateId);

    return id.ToGuid();
  }
}
