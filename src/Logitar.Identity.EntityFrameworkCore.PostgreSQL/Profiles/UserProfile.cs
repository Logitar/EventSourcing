using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Users;
using System.Text.Json;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// The profile used to configure mapping of users.
/// </summary>
internal class UserProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UserProfile"/> class.
  /// </summary>
  public UserProfile()
  {
    CreateMap<UserEntity, User>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(y => new AggregateId(y.AggregateId).ToGuid()))
      .ForMember(x => x.CustomAttributes, x => x.MapFrom(GetCustomAttributes));
    CreateMap<ExternalIdentifierEntity, ExternalIdentifier>()
      .ForMember(x => x.UpdatedOn, x => x.MapFrom(y => y.UpdatedOn ?? y.CreatedOn));
  }

  /// <summary>
  /// Resolves the custom attributes from the specified user entity.
  /// </summary>
  /// <param name="entity">The user entity.</param>
  /// <param name="user">The user output model.</param>
  /// <returns>The custom attributes.</returns>
  /// <exception cref="InvalidOperationException">The custom attributes could not be deserialized.</exception>
  private static IEnumerable<CustomAttribute> GetCustomAttributes(UserEntity entity, User user)
  {
    if (entity.CustomAttributes == null)
    {
      return Enumerable.Empty<CustomAttribute>();
    }

    Dictionary<string, string> customAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.CustomAttributes)
      ?? throw new InvalidOperationException($"The custom attributes could not be deserialized on user 'Id={entity.RealmId}'.");

    return customAttributes.Select(customAttribute => new CustomAttribute
    {
      Key = customAttribute.Key,
      Value = customAttribute.Value
    });
  }
}
