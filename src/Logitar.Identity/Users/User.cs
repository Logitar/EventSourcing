using Logitar.Identity.Realms;
using Logitar.Identity.Roles;

namespace Logitar.Identity.Users;

/// <summary>
/// The output representation of a user.
/// </summary>
public record User : Aggregate
{
  /// <summary>
  /// Gets or sets or sets the identifier of the user.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets or sets the realm in which this user belongs.
  /// </summary>
  public Realm? Realm { get; set; }

  /// <summary>
  /// Gets or sets the unique name of the user.
  /// </summary>
  public string Username { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the date and time the password changed lastly.
  /// </summary>
  public DateTime? PasswordChangedOn { get; set; }
  /// <summary>
  /// Gets or sets a value indicating whether or not the user has a password.
  /// </summary>
  public bool HasPassword { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the user account was disabled.
  /// </summary>
  public DateTime? DisabledOn { get; set; }
  /// <summary>
  /// Gets or sets a value indicating whether or not the user account is disabled.
  /// </summary>
  public bool IsDisabled { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the user signed-in lastly.
  /// </summary>
  public DateTime? SignedInOn { get; set; }

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
  /// Gets or sets the full name of the user.
  /// </summary>
  public string? FullName { get; set; }
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
  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();

  /// <summary>
  /// Gets or sets the list of external identifiers of the user.
  /// </summary>
  public IEnumerable<ExternalIdentifier> ExternalIdentifiers { get; set; } = Enumerable.Empty<ExternalIdentifier>();

  /// <summary>
  /// Gets or sets the list of roles of the user.
  /// </summary>
  public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();
}
