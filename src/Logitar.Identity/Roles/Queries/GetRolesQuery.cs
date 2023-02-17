using MediatR;

namespace Logitar.Identity.Roles.Queries;

/// <summary>
/// The query raised to retrieve a list of roles.
/// </summary>
/// <param name="Realm">The identifier or unique name of the realm to filter by.</param>
/// <param name="Search">The text to search.</param>
/// <param name="Sort">The sort value.</param>
/// <param name="IsDescending">If true, the sort will be inverted.</param>
/// <param name="Skip">The number of roles to skip.</param>
/// <param name="Take">The number of roles to return.</param>
internal record GetRolesQuery(string? Realm, string? Search, RoleSort? Sort, bool IsDescending,
  int? Skip, int? Take) : IRequest<PagedList<Role>>;
