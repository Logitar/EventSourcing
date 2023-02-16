using MediatR;

namespace Logitar.Identity.Users.Queries;

/// <summary>
/// The query raised to retrieve a list of users.
/// </summary>
/// <param name="IsDisabled">The value filtering users on their disabled status.</param>
/// <param name="Realm">The identifier or unique name of the realm to filter by.</param>
/// <param name="Search">The text to search.</param>
/// <param name="Sort">The sort value.</param>
/// <param name="IsDescending">If true, the sort will be inverted.</param>
/// <param name="Skip">The number of users to skip.</param>
/// <param name="Take">The number of users to return.</param>
internal record GetUsersQuery(bool? IsDisabled, string? Realm, string? Search,
  UserSort? Sort, bool IsDescending, int? Skip, int? Take) : IRequest<PagedList<User>>;
