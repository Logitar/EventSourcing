using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.ApiKeys.Events;

/// <summary>
/// Represents the event raised when an <see cref="ApiKeyAggregate"/> is deleted.
/// </summary>
public record ApiKeyDeletedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyDeletedEvent"/> class.
  /// </summary>
  public ApiKeyDeletedEvent()
  {
    DeleteAction = DeleteAction.Delete;
  }
}
