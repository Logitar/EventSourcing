namespace Logitar.Identity.Realms;

/// <summary>
/// Represents the possible sort values for realms.
/// </summary>
public enum RealmSort
{
  /// <summary>
  /// The realms will be sorted by their display name, or unique name if it is null.
  /// </summary>
  DisplayName,

  /// <summary>
  /// The realms will be sorted by their unique name.
  /// </summary>
  UniqueName,

  /// <summary>
  /// The realms will be sorted by their latest update date and time, including creation.
  /// </summary>
  UpdatedOn
}
