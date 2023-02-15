using MediatR;

namespace Logitar.Identity.Realms.Queries;

/// <summary>
/// The handler for the <see cref="GetRealmsQuery"/> queries.
/// </summary>
internal class GetRealmsQueryHandler : IRequestHandler<GetRealmsQuery, PagedList<Realm>>
{
  /// <summary>
  /// The realm querier.
  /// </summary>
  private readonly IRealmQuerier _realmQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetRealmsQueryHandler"/> class.
  /// </summary>
  /// <param name="realmQuerier">The realm querier.</param>
  public GetRealmsQueryHandler(IRealmQuerier realmQuerier)
  {
    _realmQuerier = realmQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of realms, or a empty collection.</returns>
  public async Task<PagedList<Realm>> Handle(GetRealmsQuery request, CancellationToken cancellationToken)
  {
    return await _realmQuerier.GetAsync(request.Search, request.Sort, request.IsDescending,
      request.Skip, request.Take, cancellationToken);
  }
}
