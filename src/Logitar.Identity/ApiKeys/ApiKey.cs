using Logitar.Identity.Realms;
using Logitar.Identity.Roles;

namespace Logitar.Identity.ApiKeys;

/// <summary>
/// The output representation of an API key.
/// </summary>
public record ApiKey : Aggregate
{
  /// <summary>
  /// Gets or sets the identifier of the API key.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the realm in which this API key belongs.
  /// </summary>
  public Realm? Realm { get; set; }

  /// <summary>
  /// Gets or sets the X-API-Key string representation of the API key.
  /// </summary>
  public string? XApiKey { get; set; }

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
  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();

  /// <summary>
  /// Gets or sets the list of roles (scopes) of the API key.
  /// </summary>
  public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();
}
