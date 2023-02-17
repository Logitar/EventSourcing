using Logitar.EventSourcing;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the base contact creation event.
/// </summary>
public abstract record ContactCreatedEvent : DomainEvent
{
  /// <summary>
  /// Gets or sets the identifier of the user owning the contact.
  /// </summary>
  public AggregateId UserId { get; init; }

  /// <summary>
  /// Gets or sets the archivation status of the contact.
  /// </summary>
  public bool IsArchived { get; init; }
  /// <summary>
  /// Gets or sets the default status of the contact.
  /// </summary>
  public bool IsDefault { get; init; }
  /// <summary>
  /// Gets or sets the verification status of the contact.
  /// </summary>
  public bool IsVerified { get; init; }

  /// <summary>
  /// Gets or sets the label describing the contact.
  /// </summary>
  public string? Label { get; init; }

  /// <summary>
  /// Gets or sets the custom attributes of the contact.
  /// </summary>
  public Dictionary<string, string> CustomAttributes { get; init; } = new();
}
