using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Users.Events;

/// <summary>
/// Represents the event raised when an <see cref="UserAggregate"/> is disabled.
/// </summary>
public record UserDisabledEvent : DomainEvent, INotification;
