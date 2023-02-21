using MediatR;

namespace Logitar.Identity.ApiKeys.Commands;

/// <summary>
/// The command raised to update an existing API key.
/// </summary>
/// <param name="Id">The identifier of the API key to update.</param>
/// <param name="Input">The update input data.</param>
internal record UpdateApiKeyCommand(Guid Id, UpdateApiKeyInput Input) : IRequest<ApiKey>;
