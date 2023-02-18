namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Defines properties of phone numbers.
/// </summary>
public interface IPhoneNumber
{
  /// <summary>
  /// Gets or sets the country code of the phone.
  /// </summary>
  string? CountryCode { get; }

  /// <summary>
  /// Gets or sets the number of the phone.
  /// </summary>
  string Number { get; }

  /// <summary>
  /// Gets or sets the extension of the phone.
  /// </summary>
  string? Extension { get; }
}
