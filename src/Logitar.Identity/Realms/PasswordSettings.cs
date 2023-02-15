namespace Logitar.Identity.Realms;

/// <summary>
/// Represents the settings used to validate a password in a realm.
/// </summary>
public record PasswordSettings
{
  /// <summary>
  /// Gets the minimum number of characters in a password.
  /// </summary>
  public int RequiredLength { get; set; }
  /// <summary>
  /// Gets the required number of unique characters in a password.
  /// </summary>
  public int RequiredUniqueChars { get; set; }
  /// <summary>
  /// If true, passwords will need to include at least one non-alphanumeric character (e.g. !"/$%?&*_+±@£¢¤¬¦²³¼½¾).
  /// </summary>
  public bool RequireNonAlphanumeric { get; set; }
  /// <summary>
  /// If true, passwords will need to include a lowercase character (a-z).
  /// </summary>
  public bool RequireLowercase { get; set; }
  /// <summary>
  /// If true, passwords will need to include an uppercase character (A-Z).
  /// </summary>
  public bool RequireUppercase { get; set; }
  /// <summary>
  /// If true, passwords will need to include a digit character (0-9).
  /// </summary>
  public bool RequireDigit { get; set; }
}
