using FluentValidation;
using Logitar.EventSourcing.Demo.Application;
using Logitar.EventSourcing.Demo.Application.Products;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.EventSourcing.Demo.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
    if (context.Exception is ValidationException validation)
    {
      base.OnException(context); // TODO(fpion): 400 BadRequest
    }
    else if (context.Exception is ProductNotFoundException productNotFound)
    {
      base.OnException(context); // TODO(fpion): 404 NotFound
    }
    else if (context.Exception is SkuAlreadyUsedException skuAlreadyUsed)
    {
      base.OnException(context); // TODO(fpion): 409 Conflict
    }
    else if (context.Exception is TooManyResultsException tooManyResults)
    {
      base.OnException(context); // TODO(fpion): 400 BadRequest
    }
    else
    {
      base.OnException(context);
    }
  }
}
