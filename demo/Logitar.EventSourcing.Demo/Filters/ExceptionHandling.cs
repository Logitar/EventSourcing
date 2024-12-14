using FluentValidation;
using Logitar.EventSourcing.Demo.Application;
using Logitar.EventSourcing.Demo.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Logitar.EventSourcing.Demo.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  private readonly ProblemDetailsFactory _problemDetailsFactory;

  public ExceptionHandling(ProblemDetailsFactory problemDetailsFactory)
  {
    _problemDetailsFactory = problemDetailsFactory;
  }

  public override void OnException(ExceptionContext context)
  {
    HttpContext httpContext = context.HttpContext;

    ProblemDetails? problemDetails = null;

    if (context.Exception is ValidationException validation)
    {
      problemDetails = _problemDetailsFactory.CreateProblemDetails(
        httpContext,
        StatusCodes.Status400BadRequest,
        "Validation",
        type: null, // NOTE(fpion): when left null, will default to the RFC HTTP Status Codes documentation.
        "Validation failed.");
      problemDetails.Populate(validation);
      problemDetails.Extensions.TryAdd("errors", validation.Errors);
    }
    else if (context.Exception is BadRequestException badRequest)
    {
      problemDetails = _problemDetailsFactory.CreateProblemDetails(httpContext, StatusCodes.Status400BadRequest);
      problemDetails.Populate(badRequest);
    }
    else if (context.Exception is NotFoundException notFound)
    {
      problemDetails = _problemDetailsFactory.CreateProblemDetails(httpContext, StatusCodes.Status404NotFound);
      problemDetails.Populate(notFound);
    }
    else if (context.Exception is ConflictException conflict)
    {
      problemDetails = _problemDetailsFactory.CreateProblemDetails(httpContext, StatusCodes.Status409Conflict);
      problemDetails.Populate(conflict);
    }

    if (problemDetails == null)
    {
      base.OnException(context);
    }
    else
    {
      problemDetails.Instance = httpContext.Request.GetDisplayUrl();

      context.Result = new ObjectResult(problemDetails)
      {
        StatusCode = problemDetails.Status
      };
      context.ExceptionHandled = true;
    }
  }
}
