using MediatR;

namespace Logitar.EventSourcing.Demo.Domain;

internal record TodoRenamed(string NewName) : DomainEvent, INotification;
