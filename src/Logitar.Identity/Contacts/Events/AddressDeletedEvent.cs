using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when an <see cref="AddressAggregate"/> is deleted.
/// </summary>
public record AddressDeletedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AddressDeletedEvent"/> class.
  /// </summary>
  public AddressDeletedEvent()
  {
    DeleteAction = DeleteAction.Delete;
  }
}
