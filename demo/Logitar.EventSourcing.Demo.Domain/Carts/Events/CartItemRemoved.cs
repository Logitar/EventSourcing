﻿using Logitar.EventSourcing.Demo.Domain.Products;
using MediatR;

namespace Logitar.EventSourcing.Demo.Domain.Carts.Events;

public record CartItemRemoved(ProductId ProductId, int Quantity) : DomainEvent, INotification;
