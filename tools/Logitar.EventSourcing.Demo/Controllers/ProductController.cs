using Logitar.EventSourcing.Demo.Application.Products.Commands;
using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Application.Products.Queries;
using Logitar.EventSourcing.Demo.Application.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.EventSourcing.Demo.Controllers;

[ApiController]
[Route("products")]
public class ProductController : ControllerBase
{
  private readonly IMediator _mediator;

  public ProductController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<ProductModel>> CreateAsync([FromBody] ProductPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceProductResult result = await _mediator.Send(new CreateOrReplaceProductCommand(Id: null, payload, Version: null), cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<ProductModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    ProductModel? product = await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
    return product == null ? NotFound() : Ok(product);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ProductModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ProductModel? product = await _mediator.Send(new ReadProductQuery(id, Sku: null), cancellationToken);
    return product == null ? NotFound() : Ok(product);
  }

  [HttpGet("sku:{sku}")]
  public async Task<ActionResult<ProductModel>> ReadAsync(string sku, CancellationToken cancellationToken)
  {
    ProductModel? product = await _mediator.Send(new ReadProductQuery(Id: null, sku), cancellationToken);
    return product == null ? NotFound() : Ok(product);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ProductModel>> ReplaceAsync(Guid id, [FromBody] ProductPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceProductResult result = await _mediator.Send(new CreateOrReplaceProductCommand(id, payload, version), cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ProductModel>>> SearchAsync(CancellationToken cancellationToken) // TODO(fpion): input parameters
  {
    SearchResults<ProductModel> products = await _mediator.Send(new SearchProductsQuery(new SearchProductsPayload()), cancellationToken);
    return Ok(products);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ProductModel>> UpdateAsync(Guid id, [FromBody] UpdateProductPayload payload, CancellationToken cancellationToken)
  {
    ProductModel? product = await _mediator.Send(new UpdateProductCommand(id, payload), cancellationToken);
    return product == null ? NotFound() : Ok(product);
  }

  private ActionResult<ProductModel> ToActionResult(CreateOrReplaceProductResult result)
  {
    if (result.Product == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri uri = new($"https://{Request.Host}/products/{result.Product.Id}", UriKind.Absolute);
      return Created(uri, result.Product);
    }

    return Ok(result.Product);
  }
}
