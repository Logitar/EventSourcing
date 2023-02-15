using Logitar.EventSourcing;

namespace Logitar.Identity.Realms;

/// <summary>
/// Exposes methods used to query realm read models.
/// </summary>
public interface IRealmQuerier
{
  /// <summary>
  /// Retrieves a realm by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm or null if not found.</returns>
  Task<Realm?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a realm by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm or null if not found.</returns>
  Task<Realm?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a realm by its unique name.
  /// </summary>
  /// <param name="uniqueName">The unique name.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm or null if not found.</returns>
  Task<Realm?> GetAsync(string uniqueName, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a list of realms using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="search">The text to search.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of realms to skip.</param>
  /// <param name="take">The number of realms to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of realms, or empty if none found.</returns>
  Task<PagedList<Realm>> GetAsync(string? search = null, RealmSort? sort = null, bool isDescending = false,
    int? skip = null, int? take = null, CancellationToken cancellationToken = default);
}
