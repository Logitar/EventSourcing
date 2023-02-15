using Logitar.Identity.Accounts;

namespace Logitar.Identity.Realms;

/// <summary>
/// Provides methods to help managing realms.
/// </summary>
internal static class RealmHelper
{
  /// <summary>
  /// Converts the specified list of custom attributes to a dictionary of custom attributes.
  /// </summary>
  /// <param name="customAttributes">The list of custom attributes.</param>
  /// <returns>The dictionary of custom attributes.</returns>
  public static Dictionary<string, string>? GetCustomAttributes(IEnumerable<CustomAttribute>? customAttributes)
  {
    if (customAttributes == null)
    {
      return null;
    }

    Dictionary<string, string> dictionary = new(capacity: customAttributes.Count());
    foreach (CustomAttribute customAttribute in customAttributes)
    {
      dictionary[customAttribute.Key] = customAttribute.Value;
    }

    return dictionary;
  }

  /// <summary>
  /// Converts the specified external provider configurations to read-only configurations.
  /// </summary>
  /// <param name="googleOAuth2Configuration">The Google OAuth 2.0 configuration.</param>
  /// <returns>The read-only configurations.</returns>
  public static Dictionary<ExternalProvider, ExternalProviderConfiguration> GetExternalProviders(
    GoogleOAuth2Configuration? googleOAuth2Configuration = null)
  {
    Dictionary<ExternalProvider, ExternalProviderConfiguration> externalProviders = new(capacity: 1);

    if (googleOAuth2Configuration != null)
    {
      externalProviders.Add(ExternalProvider.GoogleOAuth2, new ReadOnlyGoogleOAuth2Configuration(googleOAuth2Configuration));
    }

    return externalProviders;
  }
}
