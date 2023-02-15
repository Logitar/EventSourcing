namespace Logitar.Identity.Realms;

/// <summary>
/// Represents a custom attribute defined on a realm to extend the basic properties.
/// </summary>
public record CustomAttribute
{
  /// <summary>
  /// Gets or sets the key of the custom attribute.
  /// </summary>
  public string Key { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the value of the custom attribute.
  /// </summary>
  public string Value { get; set; } = string.Empty;
}
