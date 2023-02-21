namespace Logitar.Identity.Contacts;

/// <summary>
/// The phone number update input data.
/// </summary>
public record UpdatePhoneInput : UpdateContactInput
{
  /// <summary>
  /// Gets or sets the country code of the phone.
  /// </summary>
  public string? CountryCode { get; set; }

  /// <summary>
  /// Gets or sets the number of the phone.
  /// </summary>
  public string Number { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the extension of the phone.
  /// </summary>
  public string? Extension { get; set; }
}
