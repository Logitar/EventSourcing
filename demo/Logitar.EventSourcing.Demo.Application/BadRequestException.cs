using Logitar.EventSourcing.Demo.Domain.Errors;

namespace Logitar.EventSourcing.Demo.Application;

public abstract class BadRequestException : ErrorException
{
  protected BadRequestException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
