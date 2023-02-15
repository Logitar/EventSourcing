using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// Provides extensions for the <see cref="ILogger{T}"/> class.
/// </summary>
public static class LoggerExtensions
{
  /// <summary>
  /// Logs the specified exception using a generic error message.
  /// </summary>
  /// <typeparam name="T">The generic type of the logger.</typeparam>
  /// <param name="logger">The logger to use.</param>
  /// <param name="exception">The exception to log.</param>
  public static void LogError<T>(this ILogger<T> logger, Exception exception)
  {
    logger.LogError(exception, "An unexpected error occurred.");
  }
}
