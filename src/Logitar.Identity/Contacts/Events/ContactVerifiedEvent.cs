using Logitar.EventSourcing;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the base contact verification event.
/// </summary>
public abstract record ContactVerifiedEvent : DomainEvent
{
  /// <summary>
  /// Gets or sets the verification status of the contact.
  /// </summary>
  public bool IsVerified { get; init; }
}
