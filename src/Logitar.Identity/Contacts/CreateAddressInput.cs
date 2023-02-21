namespace Logitar.Identity.Contacts;

/// <summary>
/// The postal address creation input data.
/// </summary>
public record CreateAddressInput : CreateContactInput
{
  /// <summary>
  /// Gets or sets the primary line of the postal address.
  /// </summary>
  public string Line1 { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the secondary line of the postal address.
  /// </summary>
  public string? Line2 { get; set; }

  /// <summary>
  /// Gets or sets the locality (or city) of the postal address.
  /// </summary>
  public string Locality { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the postal code of the postal address.
  /// </summary>
  public string? PostalCode { get; set; }

  /// <summary>
  /// Gets or sets the country of the postal address.
  /// </summary>
  public string Country { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the region of the postal address.
  /// </summary>
  public string? Region { get; set; }
}
