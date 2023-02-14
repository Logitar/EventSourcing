using MediatR;

namespace Logitar.EventSourcing.Demo.Domain;

internal record TodoCreated(string Name) : DomainEvent, INotification;
