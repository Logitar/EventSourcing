using MediatR;

namespace Logitar.Identity.Contacts.Queries;

/// <summary>
/// The handler for the <see cref="GetPhonesQuery"/> queries.
/// </summary>
internal class GetPhonesQueryHandler : IRequestHandler<GetPhonesQuery, PagedList<Phone>>
{
  /// <summary>
  /// The phone number querier.
  /// </summary>
  private readonly IPhoneQuerier _phoneQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetPhonesQueryHandler"/> class.
  /// </summary>
  /// <param name="phoneQuerier">The phone number querier.</param>
  public GetPhonesQueryHandler(IPhoneQuerier phoneQuerier)
  {
    _phoneQuerier = phoneQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of phone numbers, or a empty collection.</returns>
  public async Task<PagedList<Phone>> Handle(GetPhonesQuery request, CancellationToken cancellationToken)
  {
    return await _phoneQuerier.GetAsync(request.IsArchived, request.IsVerified, request.Search, request.UserId,
      request.Sort, request.IsDescending, request.Skip, request.Take, cancellationToken);
  }
}
