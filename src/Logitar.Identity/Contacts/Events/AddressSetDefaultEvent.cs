using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when an <see cref="AddressAggregate"/> is set default.
/// </summary>
public record AddressSetDefaultEvent : ContactSetDefaultEvent, INotification;
