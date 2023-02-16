using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Users.Events;

/// <summary>
/// Represents the event raised when a <see cref="UserAggregate"/> is deleted.
/// </summary>
public record UserDeletedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UserDeletedEvent"/> clss.
  /// </summary>
  public UserDeletedEvent()
  {
    DeleteAction = DeleteAction.Delete;
  }
}
