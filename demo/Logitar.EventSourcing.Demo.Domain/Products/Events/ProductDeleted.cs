using MediatR;

namespace Logitar.EventSourcing.Demo.Domain.Products.Events;

public record ProductDeleted : DomainEvent, IDeleteEvent, INotification;
