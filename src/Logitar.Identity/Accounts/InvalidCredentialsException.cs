namespace Logitar.Identity.Accounts;

/// <summary>
/// The exception thrown when invalid credentials are provided.
/// </summary>
public class InvalidCredentialsException : Exception
{
  private const string DefaultMessage = "The specified credentials did not match";

  /// <summary>
  /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class.
  /// </summary>
  public InvalidCredentialsException() : this(DefaultMessage)
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class using the specified arguments.
  /// </summary>
  /// <param name="message">The exception message.</param>
  public InvalidCredentialsException(string message) : this(message, innerException: null)
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class using the specified arguments.
  /// </summary>
  /// <param name="innerException">The inner exception.</param>
  public InvalidCredentialsException(Exception innerException) : this(DefaultMessage, innerException)
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class using the specified arguments.
  /// </summary>
  /// <param name="message">The exception message.</param>
  /// <param name="innerException">The inner exception.</param>
  public InvalidCredentialsException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
