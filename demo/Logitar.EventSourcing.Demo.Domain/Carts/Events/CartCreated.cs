using MediatR;

namespace Logitar.EventSourcing.Demo.Domain.Carts.Events;

public record CartCreated : DomainEvent, INotification;
