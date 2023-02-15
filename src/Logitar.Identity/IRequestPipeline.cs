using MediatR;

namespace Logitar.Identity;

/// <summary>
/// Represents a pipeline allowing to perform tasks before and after executing a request.
/// </summary>
internal interface IRequestPipeline
{
  /// <summary>
  /// Executes the specified request and returns a response.
  /// </summary>
  /// <typeparam name="T">The type of the response.</typeparam>
  /// <param name="request">The request to execute.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The response.</returns>
  Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
}
