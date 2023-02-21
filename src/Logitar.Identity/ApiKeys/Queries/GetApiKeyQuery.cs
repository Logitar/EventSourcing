using MediatR;

namespace Logitar.Identity.ApiKeys.Queries;

/// <summary>
/// The query raised to retrieve a single API key.
/// </summary>
/// <param name="Id">The identifier of the API key.</param>
internal record GetApiKeyQuery(Guid? Id) : IRequest<ApiKey?>;
