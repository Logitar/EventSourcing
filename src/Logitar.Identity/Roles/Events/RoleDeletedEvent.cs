using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Roles.Events;

/// <summary>
/// Represents the event raised when a <see cref="RoleAggregate"/> is deleted.
/// </summary>
public record RoleDeletedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RoleDeletedEvent"/> class.
  /// </summary>
  public RoleDeletedEvent()
  {
    DeleteAction = DeleteAction.Delete;
  }
}
