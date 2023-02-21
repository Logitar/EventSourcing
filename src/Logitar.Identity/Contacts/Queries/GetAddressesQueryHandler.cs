using MediatR;

namespace Logitar.Identity.Contacts.Queries;

/// <summary>
/// The handler for the <see cref="GetAddresssQuery"/> queries.
/// </summary>
internal class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, PagedList<Address>>
{
  /// <summary>
  /// The postal address querier.
  /// </summary>
  private readonly IAddressQuerier _addressQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetAddresssQueryHandler"/> class.
  /// </summary>
  /// <param name="addressQuerier">The postal address querier.</param>
  public GetAddressesQueryHandler(IAddressQuerier addressQuerier)
  {
    _addressQuerier = addressQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of postal addresses, or a empty collection.</returns>
  public async Task<PagedList<Address>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
  {
    return await _addressQuerier.GetAsync(request.IsArchived, request.IsVerified, request.Search, request.UserId,
      request.Sort, request.IsDescending, request.Skip, request.Take, cancellationToken);
  }
}
