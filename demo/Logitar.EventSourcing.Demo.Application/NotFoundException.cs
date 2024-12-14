using Logitar.EventSourcing.Demo.Domain.Errors;

namespace Logitar.EventSourcing.Demo.Application;

public abstract class NotFoundException : ErrorException
{
  protected NotFoundException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
