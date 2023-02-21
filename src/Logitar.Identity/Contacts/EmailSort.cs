namespace Logitar.Identity.Contacts;

/// <summary>
/// Represents the possible sort values for email addresses.
/// </summary>
public enum EmailSort
{
  /// <summary>
  /// The emails will be sorted by their address.
  /// </summary>
  Address,

  /// <summary>
  /// The email addresses will be sorted by their descriptive label.
  /// </summary>
  Label,

  /// <summary>
  /// The email addresses will be sorted by their latest update date and time, including creation.
  /// </summary>
  UpdatedOn,

  /// <summary>
  /// The email addresses will be sorted by their verification date.
  /// </summary>
  VerifiedOn
}
