using Logitar.EventSourcing;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the base contact set default event.
/// </summary>
public abstract record ContactSetDefaultEvent : DomainEvent
{
  /// <summary>
  /// Gets or sets the default status of the contact.
  /// </summary>
  public bool IsDefault { get; init; }
}
