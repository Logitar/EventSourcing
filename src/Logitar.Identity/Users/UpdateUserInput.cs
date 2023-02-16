namespace Logitar.Identity.Users;

/// <summary>
/// The user update input data.
/// </summary>
public record UpdateUserInput
{
  /// <summary>
  /// Gets or sets the password of the user. If supplied, the password will be forced changed.
  /// If null, the password won't be changed.
  /// </summary>
  public string? Password { get; set; }

  /// <summary>
  /// Gets or sets the first name(s) or given name(s) of the user.
  /// </summary>
  public string? FirstName { get; set; }
  /// <summary>
  /// Gets or sets the middle name(s) of the user.
  /// </summary>
  public string? MiddleName { get; set; }
  /// <summary>
  /// Gets or sets the last name(s) or surname(s) of the user.
  /// </summary>
  public string? LastName { get; set; }
  /// <summary>
  /// Gets or sets the nickname(s) or casual name(s) or the user.
  /// </summary>
  public string? Nickname { get; set; }

  /// <summary>
  /// Gets or sets the birtdate of the user.
  /// </summary>
  public DateTime? Birthdate { get; set; }
  /// <summary>
  /// Gets or sets the gender of the user.
  /// </summary>
  public string? Gender { get; set; }

  /// <summary>
  /// Gets or sets the locale of the user.
  /// </summary>
  public string? Locale { get; set; }
  /// <summary>
  /// Gets or sets the time zone of the user. It should match the name of a time zone in the tz database.
  /// </summary>
  public string? TimeZone { get; set; }

  /// <summary>
  /// Gets or sets a link (URL) to the picture of the user.
  /// </summary>
  public string? Picture { get; set; }
  /// <summary>
  /// Gets or sets a link (URL) to the profile of the user.
  /// </summary>
  public string? Profile { get; set; }
  /// <summary>
  /// Gets or sets a link (URL) to the website of the user.
  /// </summary>
  public string? Website { get; set; }

  /// <summary>
  /// Gets or sets the custom attributes of the user.
  /// </summary>
  public IEnumerable<CustomAttribute>? CustomAttributes { get; set; }

  /// <summary>
  /// Gets or sets the role identifiers of the user.
  /// </summary>
  public IEnumerable<Guid>? Roles { get; set; }
}
