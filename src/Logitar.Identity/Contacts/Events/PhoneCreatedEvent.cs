using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when a new <see cref="PhoneAggregate"/> is created.
/// </summary>
public record PhoneCreatedEvent : ContactCreatedEvent, INotification, IPhoneNumber
{
  /// <summary>
  /// Gets or sets the country code of the phone.
  /// </summary>
  public string? CountryCode { get; init; }

  /// <summary>
  /// Gets or sets the number of the phone.
  /// </summary>
  public string Number { get; init; } = string.Empty;

  /// <summary>
  /// Gets or sets the extension of the phone.
  /// </summary>
  public string? Extension { get; init; }
}
