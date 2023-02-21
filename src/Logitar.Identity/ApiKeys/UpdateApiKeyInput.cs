namespace Logitar.Identity.ApiKeys;

/// <summary>
/// The API key creation input data.
/// </summary>
public record UpdateApiKeyInput
{
  /// <summary>
  /// Gets or sets the title (or display name) of the API key.
  /// </summary>
  public string Title { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets a textual description for the API key.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Gets or sets the custom attributes of the API key.
  /// </summary>
  public IEnumerable<CustomAttribute>? CustomAttributes { get; set; }

  /// <summary>
  /// Gets or sets the role (scope) identifiers of the API key.
  /// </summary>
  public IEnumerable<Guid>? Roles { get; set; }
}
