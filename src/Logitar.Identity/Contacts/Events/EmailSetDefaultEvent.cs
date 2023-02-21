using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when an <see cref="EmailAggregate"/> is set default.
/// </summary>
public record EmailSetDefaultEvent : ContactSetDefaultEvent, INotification;
