using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Queries;

/// <summary>
/// The handler for <see cref="GetEmailQuery"/> queries.
/// </summary>
internal class GetEmailQueryHandler : IRequestHandler<GetEmailQuery, Email?>
{
  /// <summary>
  /// The email address querier.
  /// </summary>
  private readonly IEmailQuerier _addressQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetEmailQueryHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="addressQuerier">The email address querier.</param>
  public GetEmailQueryHandler(IEmailQuerier addressQuerier)
  {
    _addressQuerier = addressQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved email address or null.</returns>
  /// <exception cref="TooManyResultsException">More than one email addresses have been found.</exception>
  public async Task<Email?> Handle(GetEmailQuery request, CancellationToken cancellationToken)
  {
    List<Email> emails = new(capacity: 2);

    if (request.Id.HasValue)
    {
      emails.AddIfNotNull(await _addressQuerier.GetAsync(request.Id.Value, cancellationToken));
    }
    if (request.UserId.HasValue)
    {
      emails.AddIfNotNull(await _addressQuerier.GetDefaultAsync(request.UserId.Value, cancellationToken));
    }

    if (emails.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return emails.SingleOrDefault();
  }
}
