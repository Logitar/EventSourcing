using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Queries;

/// <summary>
/// The handler for <see cref="GetPhoneQuery"/> queries.
/// </summary>
internal class GetPhoneQueryHandler : IRequestHandler<GetPhoneQuery, Phone?>
{
  /// <summary>
  /// The phone number querier.
  /// </summary>
  private readonly IPhoneQuerier _addressQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetPhoneQueryHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="addressQuerier">The phone number querier.</param>
  public GetPhoneQueryHandler(IPhoneQuerier addressQuerier)
  {
    _addressQuerier = addressQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved phone number or null.</returns>
  /// <exception cref="TooManyResultsException">More than one phone numbers have been found.</exception>
  public async Task<Phone?> Handle(GetPhoneQuery request, CancellationToken cancellationToken)
  {
    List<Phone> phones = new(capacity: 2);

    if (request.Id.HasValue)
    {
      phones.AddIfNotNull(await _addressQuerier.GetAsync(request.Id.Value, cancellationToken));
    }
    if (request.UserId.HasValue)
    {
      phones.AddIfNotNull(await _addressQuerier.GetDefaultAsync(request.UserId.Value, cancellationToken));
    }

    if (phones.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return phones.SingleOrDefault();
  }
}
