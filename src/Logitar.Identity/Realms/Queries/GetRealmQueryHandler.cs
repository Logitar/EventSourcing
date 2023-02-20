using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Realms.Queries;

/// <summary>
/// The handler for <see cref="GetRealmQuery"/> queries.
/// </summary>
internal class GetRealmQueryHandler : IRequestHandler<GetRealmQuery, Realm?>
{
  /// <summary>
  /// The realm querier.
  /// </summary>
  private readonly IRealmQuerier _realmQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetRealmQueryHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="realmQuerier">The realm querier.</param>
  public GetRealmQueryHandler(IRealmQuerier realmQuerier)
  {
    _realmQuerier = realmQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved realm or null.</returns>
  /// <exception cref="TooManyResultsException">More than one realms have been found.</exception>
  public async Task<Realm?> Handle(GetRealmQuery request, CancellationToken cancellationToken)
  {
    List<Realm> realms = new(capacity: 2);

    if (request.Id.HasValue)
    {
      realms.AddIfNotNull(await _realmQuerier.GetAsync(request.Id.Value, cancellationToken));
    }
    if (request.UniqueName != null)
    {
      realms.AddIfNotNull(await _realmQuerier.GetAsync(request.UniqueName, cancellationToken));
    }

    if (realms.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return realms.SingleOrDefault();
  }
}
