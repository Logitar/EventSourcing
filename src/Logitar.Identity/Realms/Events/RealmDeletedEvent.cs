using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Realms.Events;

/// <summary>
/// Represents the event raised when a <see cref="RealmAggregate"/> is deleted.
/// </summary>
public record RealmDeletedEvent : DomainEvent, INotification
{
  public RealmDeletedEvent()
  {
    DeleteAction = DeleteAction.Delete;
  }
}
