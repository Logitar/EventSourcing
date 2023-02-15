namespace Logitar.Identity.Roles;

/// <summary>
/// The role creation input data.
/// </summary>
public record CreateRoleInput
{
  /// <summary>
  /// Gets or sets the identifier of the realm in which this role belongs.
  /// </summary>
  public Guid RealmId { get; set; }

  /// <summary>
  /// Gets or sets the unique name of the role (not case-sensitive).
  /// </summary>
  public string UniqueName { get; set; } = string.Empty;
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
