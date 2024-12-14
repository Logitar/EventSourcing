using Logitar.EventSourcing.Demo.Domain.Products;
using MediatR;

namespace Logitar.EventSourcing.Demo.Domain.Carts.Events;

public record CartItemUpdated(ProductId ProductId, int Quantity) : DomainEvent, INotification;
