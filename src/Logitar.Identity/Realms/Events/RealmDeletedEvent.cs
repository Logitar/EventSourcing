using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Realms.Events;

/// <summary>
/// Represents the event raised when a <see cref="RealmAggregate"/> is deleted.
/// </summary>
public record RealmDeletedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RealmDeletedEvent"/> class.
  /// </summary>
  public RealmDeletedEvent()
  {
    DeleteAction = DeleteAction.Delete;
  }
}
