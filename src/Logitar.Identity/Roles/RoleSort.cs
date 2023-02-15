namespace Logitar.Identity.Roles;

/// <summary>
/// Represents the possible sort values for roles.
/// </summary>
public enum RoleSort
{
  /// <summary>
  /// The roles will be sorted by their display name, or unique name if it is null.
  /// </summary>
  DisplayName,

  /// <summary>
  /// The roles will be sorted by their unique name.
  /// </summary>
  UniqueName,

  /// <summary>
  /// The roles will be sorted by their latest update date and time, including creation.
  /// </summary>
  UpdatedOn
}
