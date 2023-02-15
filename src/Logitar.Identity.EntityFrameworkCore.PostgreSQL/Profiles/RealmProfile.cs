using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.Accounts;
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
  /// The key used to cache external provider configurations in the mapping context items.
  /// </summary>
  private const string ExternalProvidersKey = "ExternalProviders";

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
  /// <param name="member">The source member.</param>
  /// <param name="context">The mapping context.</param>
  /// <returns>The Google OAuth 2.0 configuration.</returns>
  /// <exception cref="InvalidOperationException">The external provider configuration type was incorrect.</exception>
  private static GoogleOAuth2Configuration? GetGoogleOAuth2Configuration(RealmEntity entity, Realm realm, GoogleOAuth2Configuration? member, ResolutionContext context)
  {
    Dictionary<ExternalProvider, ExternalProviderConfiguration> externalProviders = GetExternalPoviders(entity, context);

    if (externalProviders.TryGetValue(ExternalProvider.GoogleOAuth2, out ExternalProviderConfiguration? configuration))
    {
      return configuration is ReadOnlyGoogleOAuth2Configuration googleConfiguration
        ? new GoogleOAuth2Configuration
        {
          ClientId = googleConfiguration.ClientId
        }
        : throw new InvalidOperationException("The GoogleOAuth2 configuration was not of type ReadOnlyGoogleOAuth2Configuration.");
    }

    return null;
  }

  /// <summary>
  /// Retrieves the cached, deserialized external provider configurations from the specified realm entity and mapping context.
  /// </summary>
  /// <param name="entity">The realm entity.</param>
  /// <param name="context">The mapping context.</param>
  /// <returns>The external provider configurations.</returns>
  /// <exception cref="InvalidOperationException">The external provider configurations could not be deserialized.</exception>
  private static Dictionary<ExternalProvider, ExternalProviderConfiguration> GetExternalPoviders(RealmEntity entity, ResolutionContext context)
  {
    if (!context.Items.TryGetValue(ExternalProvidersKey, out object? value) || value == null
      || value is not Dictionary<ExternalProvider, ExternalProviderConfiguration> externalProviders)
    {
      externalProviders = entity.ExternalProviders == null
        ? new Dictionary<ExternalProvider, ExternalProviderConfiguration>()
        : (JsonSerializer.Deserialize<Dictionary<ExternalProvider, ExternalProviderConfiguration>>(entity.ExternalProviders)
          ?? throw new InvalidOperationException($"The external providers could not be deserialized on realm 'Id={entity.RealmId}'."));

      context.Items[ExternalProvidersKey] = externalProviders;
    }

    return externalProviders;
  }
}
