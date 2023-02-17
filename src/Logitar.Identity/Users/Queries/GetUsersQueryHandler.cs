using MediatR;

namespace Logitar.Identity.Users.Queries;

/// <summary>
/// The handler for the <see cref="GetUsersQuery"/> queries.
/// </summary>
internal class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedList<User>>
{
  /// <summary>
  /// The user querier.
  /// </summary>
  private readonly IUserQuerier _userQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetUsersQueryHandler"/> class.
  /// </summary>
  /// <param name="userQuerier">The user querier.</param>
  public GetUsersQueryHandler(IUserQuerier userQuerier)
  {
    _userQuerier = userQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of users, or a empty collection.</returns>
  public async Task<PagedList<User>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
  {
    return await _userQuerier.GetAsync(request.IsDisabled, request.Realm, request.Search,
      request.Sort, request.IsDescending, request.Skip, request.Take, cancellationToken);
  }
}
