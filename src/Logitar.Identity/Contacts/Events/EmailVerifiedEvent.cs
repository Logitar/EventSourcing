using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when an <see cref="EmailAggregate"/> is verified.
/// </summary>
public record EmailVerifiedEvent : ContactVerifiedEvent, INotification;
