namespace Logitar.Identity.Realms;

/// <summary>
/// Represents the settings used to validate an username in a realm.
/// </summary>
public record UsernameSettings
{
  /// <summary>
  /// Gets the list of allowed characters in an username.
  /// </summary>
  public string? AllowedCharacters { get; set; }
}
