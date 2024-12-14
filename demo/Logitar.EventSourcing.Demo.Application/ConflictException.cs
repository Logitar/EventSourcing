using Logitar.EventSourcing.Demo.Domain.Errors;

namespace Logitar.EventSourcing.Demo.Application;

public abstract class ConflictException : ErrorException
{
  protected ConflictException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
