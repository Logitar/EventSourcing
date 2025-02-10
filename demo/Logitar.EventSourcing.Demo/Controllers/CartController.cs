using FluentValidation;
using FluentValidation.Results;
using Logitar.EventSourcing.Demo.Application.Carts.Commands;
using Logitar.EventSourcing.Demo.Application.Carts.Models;
using Logitar.EventSourcing.Demo.Application.Carts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.EventSourcing.Demo.Controllers;

[ApiController]
[Route("carts")]
public class CartController : ControllerBase
{
  private readonly IMediator _mediator;

  public CartController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost("new/items/{productId}")]
  public async Task<ActionResult<CartModel>> AddItemAsync(Guid productId, [FromBody] QuantityPayload payload, CancellationToken cancellationToken)
  {
    AddItemToCartResult result = await _mediator.Send(new AddItemToCartCommand(CartId: null, productId, payload), cancellationToken);
    return ToActionResult(result);
  }

  [HttpPut("{cartId}/items/{productId}")]
  public async Task<ActionResult<CartModel>> AddOrRemoveItemAsync(Guid cartId, Guid productId, [FromBody] QuantityPayload payload, CancellationToken cancellationToken)
  {
    if (payload.Quantity > 0)
    {
      AddItemToCartResult result = await _mediator.Send(new AddItemToCartCommand(cartId, productId, payload), cancellationToken);
      return ToActionResult(result);
    }
    else if (payload.Quantity < 0)
    {
      QuantityPayload removePayload = new(payload.Quantity * -1);
      CartModel? cart = await _mediator.Send(new RemoveItemFromCartCommand(cartId, productId, removePayload), cancellationToken);
      return cart == null ? NotFound() : Ok(cart);
    }

    ValidationFailure failure = new(nameof(payload.Quantity), "The value cannot be 0.", payload.Quantity)
    {
      ErrorCode = "InvalidQuantity"
    };
    throw new ValidationException([failure]);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<CartModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CartModel? cart = await _mediator.Send(new ReadCartQuery(id), cancellationToken);
    return cart == null ? NotFound() : Ok(cart);
  }

  [HttpDelete("{cartId}/items/{productId}")]
  public async Task<ActionResult<CartModel>> RemoveItemAsync(Guid cartId, Guid productId, CancellationToken cancellationToken)
  {
    CartModel? cart = await _mediator.Send(new RemoveItemFromCartCommand(cartId, productId, new QuantityPayload()), cancellationToken);
    return cart == null ? NotFound() : Ok(cart);
  }

  [HttpPatch("{cartId}/items/{productId}")]
  public async Task<ActionResult<CartModel>> UpdateItemAsync(Guid cartId, Guid productId, [FromBody] QuantityPayload payload, CancellationToken cancellationToken)
  {
    CartModel? cart = await _mediator.Send(new UpdateCartItemCommand(cartId, productId, payload), cancellationToken);
    return cart == null ? NotFound() : Ok(cart);
  }

  private ActionResult<CartModel> ToActionResult(AddItemToCartResult result)
  {
    if (result.Created)
    {
      Uri uri = new($"https://{Request.Host}/carts/{result.Cart.Id}", UriKind.Absolute);
      return Created(uri, result.Cart);
    }

    return Ok(result.Cart);
  }
}
