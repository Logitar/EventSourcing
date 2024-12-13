using MediatR;

namespace Logitar.EventSourcing.Demo.Domain.Products.Events;

public record ProductCreated(Sku Sku) : DomainEvent, INotification;
