namespace Logitar.Identity.Contacts;

/// <summary>
/// Represents the possible sort values for phone numbers.
/// </summary>
public enum PhoneSort
{
  /// <summary>
  /// The phone numbers will be sorted by their descriptive label.
  /// </summary>
  Label,

  /// <summary>
  /// The phones will be sorted by their number.
  /// </summary>
  Number,

  /// <summary>
  /// The phone numbers will be sorted by their latest update date and time, including creation.
  /// </summary>
  UpdatedOn,

  /// <summary>
  /// The phone numbers will be sorted by their verification date.
  /// </summary>
  VerifiedOn
}
