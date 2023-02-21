using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Roles.Queries;

/// <summary>
/// The handler for <see cref="GetRoleQuery"/> queries.
/// </summary>
internal class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, Role?>
{
  /// <summary>
  /// The role querier.
  /// </summary>
  private readonly IRoleQuerier _roleQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetRoleQueryHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="roleQuerier">The role querier.</param>
  public GetRoleQueryHandler(IRoleQuerier roleQuerier)
  {
    _roleQuerier = roleQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved role or null.</returns>
  /// <exception cref="TooManyResultsException">More than one roles have been found.</exception>
  public async Task<Role?> Handle(GetRoleQuery request, CancellationToken cancellationToken)
  {
    List<Role> roles = new(capacity: 2);

    if (request.Id.HasValue)
    {
      roles.AddIfNotNull(await _roleQuerier.GetAsync(request.Id.Value, cancellationToken));
    }
    if (request.Realm != null && request.UniqueName != null)
    {
      roles.AddIfNotNull(await _roleQuerier.GetAsync(request.Realm, request.UniqueName, cancellationToken));
    }

    if (roles.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return roles.SingleOrDefault();
  }
}
