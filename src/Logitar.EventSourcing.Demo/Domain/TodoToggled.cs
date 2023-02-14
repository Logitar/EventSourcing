using MediatR;

namespace Logitar.EventSourcing.Demo.Domain;

internal record TodoToggled(bool IsCompleted) : DomainEvent, INotification;
