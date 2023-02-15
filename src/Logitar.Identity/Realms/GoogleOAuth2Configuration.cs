namespace Logitar.Identity.Realms;

/// <summary>
/// Represents the configuration of the Google OAuth 2.0 external authentication provider.
/// </summary>
public record GoogleOAuth2Configuration
{
  /// <summary>
  /// Gets the client identifier used for the external provider.
  /// </summary>
  public string ClientId { get; set; } = string.Empty;
}
