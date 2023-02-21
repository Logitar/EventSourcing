using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Queries;

/// <summary>
/// The handler for <see cref="GetAddressQuery"/> queries.
/// </summary>
internal class GetAddressQueryHandler : IRequestHandler<GetAddressQuery, Address?>
{
  /// <summary>
  /// The postal address querier.
  /// </summary>
  private readonly IAddressQuerier _addressQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetAddressQueryHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="addressQuerier">The postal address querier.</param>
  public GetAddressQueryHandler(IAddressQuerier addressQuerier)
  {
    _addressQuerier = addressQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved postal address or null.</returns>
  /// <exception cref="TooManyResultsException">More than one postal addresses have been found.</exception>
  public async Task<Address?> Handle(GetAddressQuery request, CancellationToken cancellationToken)
  {
    List<Address> addresses = new(capacity: 2);

    if (request.Id.HasValue)
    {
      addresses.AddIfNotNull(await _addressQuerier.GetAsync(request.Id.Value, cancellationToken));
    }
    if (request.UserId.HasValue)
    {
      addresses.AddIfNotNull(await _addressQuerier.GetDefaultAsync(request.UserId.Value, cancellationToken));
    }

    if (addresses.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return addresses.SingleOrDefault();
  }
}
