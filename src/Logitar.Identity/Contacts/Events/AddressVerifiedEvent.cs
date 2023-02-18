using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when an <see cref="AddressAggregate"/> is verified.
/// </summary>
public record AddressVerifiedEvent : ContactVerifiedEvent, INotification;
