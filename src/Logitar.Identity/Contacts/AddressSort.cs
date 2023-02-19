namespace Logitar.Identity.Contacts;

/// <summary>
/// Represents the possible sort values for postal addresses.
/// </summary>
public enum AddressSort
{
  /// <summary>
  /// The postal addresses will be sorted by their country component.
  /// </summary>
  Country,

  /// <summary>
  /// The postal addresses will be sorted by their descriptive label.
  /// </summary>
  Label,

  /// <summary>
  /// The postal addresses will be sorted by their primary line component.
  /// </summary>
  Line1,

  /// <summary>
  /// The postal addresses will be sorted by their locality component.
  /// </summary>
  Locality,

  /// <summary>
  /// The postal addresses will be sorted by their latest update date and time, including creation.
  /// </summary>
  UpdatedOn,

  /// <summary>
  /// The postal addresses will be sorted by their verification date.
  /// </summary>
  VerifiedOn
}
