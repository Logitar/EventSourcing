using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Users.Events;

/// <summary>
/// Represents the event raised when an <see cref="UserAggregate"/> is enabled.
/// </summary>
public record UserEnabledEvent : DomainEvent, INotification;
