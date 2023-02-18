using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when a new <see cref="AddressAggregate"/> is created.
/// </summary>
public record AddressCreatedEvent : ContactCreatedEvent, INotification
{
  /// <summary>
  /// Gets or sets the primary line of the postal address.
  /// </summary>
  public string Line1 { get; init; } = string.Empty;

  /// <summary>
  /// Gets or sets the secondary line of the postal address.
  /// </summary>
  public string? Line2 { get; init; }

  /// <summary>
  /// Gets or sets the locality (or city) of the postal address.
  /// </summary>
  public string Locality { get; init; } = string.Empty;

  /// <summary>
  /// Gets or sets the postal code of the postal address.
  /// </summary>
  public string? PostalCode { get; init; }

  /// <summary>
  /// Gets or sets the country of the postal address.
  /// </summary>
  public string Country { get; init; } = string.Empty;

  /// <summary>
  /// Gets or sets the region of the postal address.
  /// </summary>
  public string? Region { get; init; }
}
