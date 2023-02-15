namespace Logitar.Identity.Realms;

/// <summary>
/// Represents the settings used to validate an username in a realm.
/// </summary>
public record ReadOnlyUsernameSettings
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ReadOnlyUsernameSettings"/> class.
  /// </summary>
  public ReadOnlyUsernameSettings()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="ReadOnlyUsernameSettings"/> class using the specified settings.
  /// </summary>
  /// <param name="settings">The username settings.</param>
  public ReadOnlyUsernameSettings(UsernameSettings settings)
  {
    AllowedCharacters = settings.AllowedCharacters;
  }

  /// <summary>
  /// Gets the list of allowed characters in an username; defaults to "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+".
  /// </summary>
  public string? AllowedCharacters { get; init; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
}
