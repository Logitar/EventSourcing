using Logitar.EventSourcing;

namespace Logitar.Identity.Contacts;

/// <summary>
/// Exposes methods used to query email address read models.
/// </summary>
public interface IEmailQuerier
{
  /// <summary>
  /// Retrieves an email address by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The email address or null if not found.</returns>
  Task<Email?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves an email address by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The email address or null if not found.</returns>
  Task<Email?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves the default email address of the specified user.
  /// </summary>
  /// <param name="id">The identifier of the user to retrieve the default email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The email address or null if not found.</returns>
  Task<Email?> GetDefaultAsync(Guid userId, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a list of email addresses using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="isArchived">The value filtering email addresses on their archivation status.</param>
  /// <param name="isVerified">The value filtering email addresses on their verification status.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="userId">The identifier of the user to filter by.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of email addresses to skip.</param>
  /// <param name="take">The number of email addresses to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of email addresses, or empty if none found.</returns>
  Task<PagedList<Email>> GetAsync(bool? isArchived = null, bool? isVerified = null, string? search = null, Guid? userId = null,
    EmailSort? sort = null, bool isDescending = false, int? skip = null, int? take = null, CancellationToken cancellationToken = default);
}
