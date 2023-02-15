using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Roles;
using System.Text.Json;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// The profile used to configure mapping of roles.
/// </summary>
internal class RoleProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RoleProfile"/> class.
  /// </summary>
  public RoleProfile()
  {
    CreateMap<RoleEntity, Role>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(y => new AggregateId(y.AggregateId).ToGuid()))
      .ForMember(x => x.CustomAttributes, x => x.MapFrom(GetCustomAttributes));
  }

  /// <summary>
  /// Resolves the custom attributes from the specified role entity.
  /// </summary>
  /// <param name="entity">The role entity.</param>
  /// <param name="role">The role output model.</param>
  /// <returns>The custom attributes.</returns>
  /// <exception cref="InvalidOperationException">The custom attributes could not be deserialized.</exception>
  private static IEnumerable<CustomAttribute> GetCustomAttributes(RoleEntity entity, Role role)
  {
    if (entity.CustomAttributes == null)
    {
      return Enumerable.Empty<CustomAttribute>();
    }

    Dictionary<string, string> customAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.CustomAttributes)
      ?? throw new InvalidOperationException($"The custom attributes could not be deserialized on role 'Id={entity.RoleId}'.");

    return customAttributes.Select(customAttribute => new CustomAttribute
    {
      Key = customAttribute.Key,
      Value = customAttribute.Value
    });
  }
}
