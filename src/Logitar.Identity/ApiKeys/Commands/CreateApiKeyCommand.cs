using MediatR;

namespace Logitar.Identity.ApiKeys.Commands;

/// <summary>
/// The command raised to create a new API key.
/// </summary>
/// <param name="Input">The creation input data.</param>
internal record CreateApiKeyCommand(CreateApiKeyInput Input) : IRequest<ApiKey>;
