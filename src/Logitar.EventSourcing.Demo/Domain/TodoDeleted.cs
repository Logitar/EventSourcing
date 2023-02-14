using MediatR;

namespace Logitar.EventSourcing.Demo.Domain;

internal record TodoDeleted : DomainEvent, INotification
{
  public TodoDeleted()
  {
    DeleteAction = DeleteAction.Delete;
  }
}
