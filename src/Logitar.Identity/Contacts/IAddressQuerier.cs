using Logitar.EventSourcing;

namespace Logitar.Identity.Contacts;

/// <summary>
/// Exposes methods used to query postal address read models.
/// </summary>
public interface IAddressQuerier
{
  /// <summary>
  /// Retrieves a postal address by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The postal address or null if not found.</returns>
  Task<Address?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a postal address by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The postal address or null if not found.</returns>
  Task<Address?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves the default postal address of the specified user.
  /// </summary>
  /// <param name="id">The identifier of the user to retrieve the default postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The postal address or null if not found.</returns>
  Task<Address?> GetDefaultAsync(Guid userId, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a list of postal addresses using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="isArchived">The value filtering postal addresses on their archivation status.</param>
  /// <param name="isVerified">The value filtering postal addresses on their verification status.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="userId">The identifier of the user to filter by.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of postal addresses to skip.</param>
  /// <param name="take">The number of postal addresses to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of postal addresses, or empty if none found.</returns>
  Task<PagedList<Address>> GetAsync(bool? isArchived = null, bool? isVerified = null, string? search = null, Guid? userId = null,
    AddressSort? sort = null, bool isDescending = false, int? skip = null, int? take = null, CancellationToken cancellationToken = default);
}
