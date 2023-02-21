using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when an <see cref="EmailAggregate"/> is deleted.
/// </summary>
public record EmailDeletedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EmailDeletedEvent"/> class.
  /// </summary>
  public EmailDeletedEvent()
  {
    DeleteAction = DeleteAction.Delete;
  }
}
