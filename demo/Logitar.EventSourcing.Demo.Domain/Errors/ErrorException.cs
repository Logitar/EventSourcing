namespace Logitar.EventSourcing.Demo.Domain.Errors;

public abstract class ErrorException : Exception
{
  public abstract Error Error { get; }

  protected ErrorException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
