namespace Logitar.Identity.Realms;

/// <summary>
/// Represents the settings used to validate a password in a realm.
/// </summary>
public record ReadOnlyPasswordSettings
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ReadOnlyPasswordSettings"/> class.
  /// </summary>
  public ReadOnlyPasswordSettings()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="ReadOnlyPasswordSettings"/> class using the specified settings.
  /// </summary>
  /// <param name="settings">The password settings.</param>
  public ReadOnlyPasswordSettings(PasswordSettings settings)
  {
    RequiredLength = settings.RequiredLength;
    RequiredUniqueChars = settings.RequiredUniqueChars;
    RequireNonAlphanumeric = settings.RequireNonAlphanumeric;
    RequireLowercase = settings.RequireLowercase;
    RequireUppercase = settings.RequireUppercase;
    RequireDigit = settings.RequireDigit;
  }

  /// <summary>
  /// Gets the minimum number of characters in a password; defaults to 6.
  /// </summary>
  public int RequiredLength { get; init; } = 6;
  /// <summary>
  /// Gets the required number of unique characters in a password; defaults to 1.
  /// </summary>
  public int RequiredUniqueChars { get; init; } = 1;
  /// <summary>
  /// If true, passwords will need to include at least one non-alphanumeric character (e.g. !"/$%?&*_+±@£¢¤¬¦²³¼½¾); defaults to false.
  /// </summary>
  public bool RequireNonAlphanumeric { get; init; } = false;
  /// <summary>
  /// If true, passwords will need to include a lowercase character (a-z); defaults to true.
  /// </summary>
  public bool RequireLowercase { get; init; } = true;
  /// <summary>
  /// If true, passwords will need to include an uppercase character (A-Z); defaults to true.
  /// </summary>
  public bool RequireUppercase { get; init; } = true;
  /// <summary>
  /// If true, passwords will need to include a digit character (0-9); defaults to true.
  /// </summary>
  public bool RequireDigit { get; init; } = true;
}
