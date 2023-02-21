using MediatR;

namespace Logitar.Identity.Contacts.Queries;

/// <summary>
/// The handler for the <see cref="GetEmailsQuery"/> queries.
/// </summary>
internal class GetEmailsQueryHandler : IRequestHandler<GetEmailsQuery, PagedList<Email>>
{
  /// <summary>
  /// The email address querier.
  /// </summary>
  private readonly IEmailQuerier _emailQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetEmailsQueryHandler"/> class.
  /// </summary>
  /// <param name="emailQuerier">The email address querier.</param>
  public GetEmailsQueryHandler(IEmailQuerier emailQuerier)
  {
    _emailQuerier = emailQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of email addresses, or a empty collection.</returns>
  public async Task<PagedList<Email>> Handle(GetEmailsQuery request, CancellationToken cancellationToken)
  {
    return await _emailQuerier.GetAsync(request.IsArchived, request.IsVerified, request.Search, request.UserId,
      request.Sort, request.IsDescending, request.Skip, request.Take, cancellationToken);
  }
}
