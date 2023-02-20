using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Users.Queries;

/// <summary>
/// The handler for <see cref="GetUserQuery"/> queries.
/// </summary>
internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, User?>
{
  /// <summary>
  /// The user querier.
  /// </summary>
  private readonly IUserQuerier _userQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetUserQueryHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="userQuerier">The user querier.</param>
  public GetUserQueryHandler(IUserQuerier userQuerier)
  {
    _userQuerier = userQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved user or null.</returns>
  /// <exception cref="TooManyResultsException">More than one users have been found.</exception>
  public async Task<User?> Handle(GetUserQuery request, CancellationToken cancellationToken)
  {
    List<User> users = new(capacity: 2);

    if (request.Id.HasValue)
    {
      users.AddIfNotNull(await _userQuerier.GetAsync(request.Id.Value, cancellationToken));
    }
    if (request.Realm != null)
    {
      if (request.Username != null)
      {
        users.AddIfNotNull(await _userQuerier.GetAsync(request.Realm, request.Username, cancellationToken));
      }
      if (request.ExternalKey != null && request.ExternalValue != null)
      {
        users.AddIfNotNull(await _userQuerier.GetAsync(request.Realm, request.ExternalKey, request.ExternalValue, cancellationToken));
      }
    }

    if (users.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return users.SingleOrDefault();
  }
}
