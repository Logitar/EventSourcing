namespace Logitar.Identity;

/// <summary>
/// The exception thrown when too many results are found.
/// </summary>
internal class TooManyResultsException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="TooManyResultsException"/> class.
  /// </summary>
  public TooManyResultsException() : base("Too many results have been found.")
  {
  }
}
