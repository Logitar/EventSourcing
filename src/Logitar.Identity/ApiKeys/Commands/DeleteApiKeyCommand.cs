using MediatR;

namespace Logitar.Identity.ApiKeys.Commands;

/// <summary>
/// The command raised to delete an API key.
/// </summary>
/// <param name="Id">The identifier of the API key to delete.</param>
internal record DeleteApiKeyCommand(Guid Id) : IRequest<ApiKey>;
