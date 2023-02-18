using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when a <see cref="PhoneAggregate"/> is set default.
/// </summary>
public record PhoneSetDefaultEvent : ContactSetDefaultEvent, INotification;
