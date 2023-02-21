namespace Logitar.Identity.ApiKeys;

/// <summary>
/// The API key creation input data.
/// </summary>
public record CreateApiKeyInput
{
  /// <summary>
  /// Gets or sets the identifier of the realm in which this API key belongs.
  /// </summary>
  public Guid RealmId { get; set; }

  /// <summary>
  /// Gets or sets the prefix of the API key, typically two characters.
  /// </summary>
  public string Prefix { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the title (or display name) of the API key.
  /// </summary>
  public string Title { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets a textual description for the API key.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the API key will expire.
  /// </summary>
  public DateTime? ExpiresOn { get; set; }

  /// <summary>
  /// Gets or sets the custom attributes of the API key.
  /// </summary>
  public IEnumerable<CustomAttribute>? CustomAttributes { get; set; }

  /// <summary>
  /// Gets or sets the role (scope) identifiers of the API key.
  /// </summary>
  public IEnumerable<Guid>? Roles { get; set; }
}
