namespace Logitar.Identity.Contacts;

/// <summary>
/// The output representation of a phone number.
/// </summary>
public record Phone : Contact
{
  /// <summary>
  /// Gets or sets or sets the identifier of the email address.
  /// </summary>
  public Guid Id { get; set; }

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
