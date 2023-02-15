namespace Logitar.Identity.Realms;

/// <summary>
/// Represents the configuration of the Google OAuth 2.0 external authentication provider.
/// </summary>
public record ReadOnlyGoogleOAuth2Configuration : ExternalProviderConfiguration
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ReadOnlyGoogleOAuth2Configuration"/> class.
  /// </summary>
  public ReadOnlyGoogleOAuth2Configuration()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="ReadOnlyGoogleOAuth2Configuration"/> class using the specified configuration.
  /// </summary>
  /// <param name="configuration">The configuration representation model.</param>
  public ReadOnlyGoogleOAuth2Configuration(GoogleOAuth2Configuration configuration)
  {
    ClientId = configuration.ClientId;
  }

  /// <summary>
  /// Gets the client identifier used for the external provider.
  /// </summary>
  public string ClientId { get; init; } = string.Empty;
}
