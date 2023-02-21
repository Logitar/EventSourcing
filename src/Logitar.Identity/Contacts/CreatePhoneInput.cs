namespace Logitar.Identity.Contacts;

/// <summary>
/// The phone number creation input data.
/// </summary>
public record CreatePhoneInput : CreateContactInput
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
