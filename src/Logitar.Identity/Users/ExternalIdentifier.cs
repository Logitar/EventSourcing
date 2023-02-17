namespace Logitar.Identity.Users;

/// <summary>
/// The output representation of an external identifier.
/// </summary>
public record ExternalIdentifier
{
  /// <summary>
  /// Gets or sets the key of the external identifier.
  /// </summary>
  public string Key { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the value of the external identifier.
  /// </summary>
  public string Value { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the date and time when the external identifier was created.
  /// </summary>
  public DateTime CreatedOn { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the external identifier was updated lastly.
  /// </summary>
  public DateTime UpdatedOn { get; set; }
}
