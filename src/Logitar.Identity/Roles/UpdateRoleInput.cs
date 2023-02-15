namespace Logitar.Identity.Roles;

/// <summary>
/// The role update input data.
/// </summary>
public record UpdateRoleInput
{
  /// <summary>
  /// Gets or sets the display name of the role.
  /// </summary>
  public string? DisplayName { get; set; }
  /// <summary>
  /// Gets or sets a textual description for the role.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Gets or sets the custom attributes of the role.
  /// </summary>
  public IEnumerable<CustomAttribute>? CustomAttributes { get; set; }
}
