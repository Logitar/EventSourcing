using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when a <see cref="PhoneAggregate"/> is verified.
/// </summary>
public record PhoneVerifiedEvent : ContactVerifiedEvent, INotification
{
}
