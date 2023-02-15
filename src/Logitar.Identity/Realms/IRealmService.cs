namespace Logitar.Identity.Realms;

/// <summary>
/// Exposes methods to manage realms in the identity system.
/// </summary>
public interface IRealmService
{
  /// <summary>
  /// Creates a new realm.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created realm.</returns>
  Task<Realm> CreateAsync(CreateRealmInput input, CancellationToken cancellationToken = default);
  /// <summary>
  /// Deletes a realm.
  /// </summary>
  /// <param name="id">The identifier of the realm.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted realm.</returns>
  Task<Realm> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a realm by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the realm.</param>
  /// <param name="uniqueName">The unique name of the realm.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm, or null if not found.</returns>
  Task<Realm?> GetAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
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
  /// <summary>
  /// Updates a realm.
  /// </summary>
  /// <param name="id">The identifier of the realm.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated realm.</returns>
  Task<Realm> UpdateAsync(Guid id, UpdateRealmInput input, CancellationToken cancellationToken = default);
}
