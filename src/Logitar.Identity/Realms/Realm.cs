namespace Logitar.Identity.Realms;

/// <summary>
/// The output representation of a realm.
/// </summary>
public record Realm : Aggregate
{
  /// <summary>
  /// Gets or sets the identifier of the realm.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the unique name of the realm (not case-sensitive).
  /// </summary>
  public string UniqueName { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the display name of the realm.
  /// </summary>
  public string? DisplayName { get; set; }
  /// <summary>
  /// Gets or sets a textual description for the realm.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Gets or sets the default locale of the realm.
  /// </summary>
  public string? DefaultLocale { get; set; }
  /// <summary>
  /// Gets or sets the URL of the realm, if it is used by an external Web application.
  /// </summary>
  public string? Url { get; set; }

  /// <summary>
  /// If true, a confirmed contact is required for the user to be able to sign-in.
  /// </summary>
  public bool RequireConfirmedAccount { get; set; }
  /// <summary>
  /// If true, primary email addresses unicity will be enforced in this realm, allowing users to log
  /// in with either their username and their primary email address.
  /// </summary>
  public bool RequireUniqueEmail { get; set; }

  /// <summary>
  /// Gets or sets the settings used to validate usernames in this realm.
  /// </summary>
  public UsernameSettings UsernameSettings { get; set; } = new();
  /// <summary>
  /// Gets or sets the settings used to validate passwords in this realm.
  /// </summary>
  public PasswordSettings PasswordSettings { get; set; } = new();

  /// <summary>
  /// Gets or sets the secret used to sign JSON Web Tokens.
  /// </summary>
  public string JwtSecret { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the configuration for the Google OAuth 2.0 authentication provider.
  /// </summary>
  public GoogleOAuth2Configuration? GoogleOAuth2Configuration { get; set; }

  /// <summary>
  /// Gets or sets the custom attributes of the realm.
  /// </summary>
  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
