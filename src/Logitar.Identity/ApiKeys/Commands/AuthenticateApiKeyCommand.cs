using MediatR;

namespace Logitar.Identity.ApiKeys.Commands;

/// <summary>
/// The command raised to authenticate an API key.
/// </summary>
/// <param name="XApiKey">The string representation of the X-API-Key.</param>
/// <param name="Prefix">The expected API key prefix.</param>
internal record AuthenticateApiKeyCommand(string XApiKey, string Prefix) : IRequest<ApiKey>;
