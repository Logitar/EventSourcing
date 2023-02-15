using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Realms;
using System.Text.Json;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// The profile used to configure mapping of realms.
/// </summary>
internal class RealmProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RealmProfile"/> class.
  /// </summary>
  public RealmProfile()
  {
    CreateMap<RealmEntity, Realm>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(y => new AggregateId(y.AggregateId).ToGuid()))
      .ForMember(x => x.UsernameSettings, x => x.MapFrom(GetUsernameSettings))
      .ForMember(x => x.PasswordSettings, x => x.MapFrom(GetPasswordSettings))
      .ForMember(x => x.GoogleOAuth2Configuration, x => x.MapFrom(GetGoogleOAuth2Configuration))
      .ForMember(x => x.CustomAttributes, x => x.MapFrom(GetCustomAttributes));
    CreateMap<ReadOnlyGoogleOAuth2Configuration, GoogleOAuth2Configuration>();
  }

  /// <summary>
  /// Resolves the custom attributes from the specified realm entity.
  /// </summary>
  /// <param name="entity">The realm entity.</param>
  /// <param name="realm">The realm output model.</param>
  /// <returns>The custom attributes.</returns>
  /// <exception cref="InvalidOperationException">The custom attributes could not be deserialized.</exception>
  private static IEnumerable<CustomAttribute> GetCustomAttributes(RealmEntity entity, Realm realm)
  {
    if (entity.CustomAttributes == null)
    {
      return Enumerable.Empty<CustomAttribute>();
    }

    Dictionary<string, string> customAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.CustomAttributes)
      ?? throw new InvalidOperationException($"The custom attributes could not be deserialized on realm 'Id={entity.RealmId}'.");

    return customAttributes.Select(customAttribute => new CustomAttribute
    {
      Key = customAttribute.Key,
      Value = customAttribute.Value
    });
  }

  /// <summary>
  /// Resolves the password settings from the specified realm entity.
  /// </summary>
  /// <param name="entity">The realm entity.</param>
  /// <param name="realm">The realm output model.</param>
  /// <returns>The password settings.</returns>
  /// <exception cref="InvalidOperationException">The password settings could not be deserialized.</exception>
  private static PasswordSettings GetPasswordSettings(RealmEntity entity, Realm realm)
  {
    return JsonSerializer.Deserialize<PasswordSettings>(entity.UsernameSettings)
      ?? throw new InvalidOperationException($"The password settings could not be deserialized on realm 'Id={entity.RealmId}'.");
  }

  /// <summary>
  /// Resolves the username settings from the specified realm entity.
  /// </summary>
  /// <param name="entity">The realm entity.</param>
  /// <param name="realm">The realm output model.</param>
  /// <returns>The username settings.</returns>
  /// <exception cref="InvalidOperationException">The username settings could not be deserialized.</exception>
  private static UsernameSettings GetUsernameSettings(RealmEntity entity, Realm realm)
  {
    return JsonSerializer.Deserialize<UsernameSettings>(entity.UsernameSettings)
      ?? throw new InvalidOperationException($"The username settings could not be deserialized on realm 'Id={entity.RealmId}'.");
  }

  /// <summary>
  /// Resolves the Google OAuth 2.0 configuration from the specified realm entity.
  /// </summary>
  /// <param name="entity">The realm entity.</param>
  /// <param name="realm">The realm output model.</param>
  /// <param name="member">The source memberl.</param>
  /// <param name="context">The mapping context.</param>
  /// <returns>The Google OAuth 2.0 configuration.</returns>
  /// <exception cref="InvalidOperationException">The external provider configuration could not be deserialized.</exception>
  private static GoogleOAuth2Configuration? GetGoogleOAuth2Configuration(RealmEntity entity, Realm realm, GoogleOAuth2Configuration? member, ResolutionContext context)
  {
    if (entity.GoogleOAuth2Configuration == null)
    {
      return null;
    }

    ReadOnlyGoogleOAuth2Configuration googleOAuth2Configuration = JsonSerializer.Deserialize<ReadOnlyGoogleOAuth2Configuration>(entity.GoogleOAuth2Configuration)
      ?? throw new InvalidOperationException($"The Google OAuth 2.0 provider authentication configuration could not be deserialized on realm 'Id={entity.RealmId}'.");

    return context.Mapper.Map<GoogleOAuth2Configuration>(googleOAuth2Configuration);
  }
}
