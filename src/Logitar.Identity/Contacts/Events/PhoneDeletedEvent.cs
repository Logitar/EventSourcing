using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when a <see cref="PhoneAggregate"/> is deleted.
/// </summary>
public record PhoneDeletedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneDeletedEvent"/> class.
  /// </summary>
  public PhoneDeletedEvent()
  {
    DeleteAction = DeleteAction.Delete;
  }
}
