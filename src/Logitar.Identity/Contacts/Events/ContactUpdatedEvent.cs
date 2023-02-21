using Logitar.EventSourcing;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the base contact update event.
/// </summary>
public abstract record ContactUpdatedEvent : DomainEvent
{
  /// <summary>
  /// Gets or sets the archivation status of the contact.
  /// </summary>
  public bool IsArchived { get; init; }
  /// <summary>
  /// Gets or sets the default status of the contact.
  /// </summary>
  public bool IsDefault { get; init; }
  /// <summary>
  /// Gets or sets the verification action performed by this event.
  /// </summary>
  public VerificationAction VerificationAction { get; init; }

  /// <summary>
  /// Gets or sets the label describing the contact.
  /// </summary>
  public string? Label { get; init; }

  /// <summary>
  /// Gets or sets the custom attributes of the contact.
  /// </summary>
  public Dictionary<string, string> CustomAttributes { get; init; } = new();
}
