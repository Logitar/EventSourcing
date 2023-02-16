using Logitar.EventSourcing;

namespace Logitar.Identity.Users;

/// <summary>
/// Exposes methods used to query user read models.
/// </summary>
public interface IUserQuerier
{
  /// <summary>
  /// Retrieves a user by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  Task<User?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a user by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a user by its realm and unique name.
  /// </summary>
  /// <param name="realmId">The identifier of the realm in which to search the unique name.</param>
  /// <param name="username">The unique name.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  Task<User?> GetAsync(Guid realmId, string username, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a list of users using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="realmId">The identifier of the realm to filter by.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of users to skip.</param>
  /// <param name="take">The number of users to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of users, or empty if none found.</returns>
  Task<PagedList<User>> GetAsync(Guid? realmId = null, string? search = null, UserSort? sort = null, bool isDescending = false,
    int? skip = null, int? take = null, CancellationToken cancellationToken = default);
}
