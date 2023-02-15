using MediatR;

namespace Logitar.Identity.Realms.Queries;

/// <summary>
/// The query raised to retrieve a list of realms.
/// </summary>
/// <param name="Search">The text to search.</param>
/// <param name="Sort">The sort value.</param>
/// <param name="IsDescending">If true, the sort will be inverted.</param>
/// <param name="Skip">The number of realms to skip.</param>
/// <param name="Take">The number of realms to return.</param>
internal record GetRealmsQuery(string? Search, RealmSort? Sort, bool IsDescending,
  int? Skip, int? Take) : IRequest<PagedList<Realm>>;
