using MediatR;

namespace Logitar.Identity.ApiKeys.Queries;

/// <summary>
/// The query raised to retrieve a list of API keys.
/// </summary>
/// <param name="Realm">The identifier or unique name of the realm to filter by.</param>
/// <param name="Search">The text to search.</param>
/// <param name="Sort">The sort value.</param>
/// <param name="IsDescending">If true, the sort will be inverted.</param>
/// <param name="Skip">The number of API keys to skip.</param>
/// <param name="Take">The number of API keys to return.</param>
internal record GetApiKeysQuery(string? Realm, string? Search, ApiKeySort? Sort, bool IsDescending,
  int? Skip, int? Take) : IRequest<PagedList<ApiKey>>;
