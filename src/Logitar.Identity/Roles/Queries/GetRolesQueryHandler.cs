using MediatR;

namespace Logitar.Identity.Roles.Queries;

/// <summary>
/// The handler for the <see cref="GetRolesQuery"/> queries.
/// </summary>
internal class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, PagedList<Role>>
{
  /// <summary>
  /// The role querier.
  /// </summary>
  private readonly IRoleQuerier _roleQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetRolesQueryHandler"/> class.
  /// </summary>
  /// <param name="roleQuerier">The role querier.</param>
  public GetRolesQueryHandler(IRoleQuerier roleQuerier)
  {
    _roleQuerier = roleQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of roles, or a empty collection.</returns>
  public async Task<PagedList<Role>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
  {
    return await _roleQuerier.GetAsync(request.Realm, request.Search, request.Sort, request.IsDescending,
      request.Skip, request.Take, cancellationToken);
  }
}
