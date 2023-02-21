using Logitar.EventSourcing;

namespace Logitar.Identity.Contacts;

/// <summary>
/// Exposes methods used to query phone number read models.
/// </summary>
public interface IPhoneQuerier
{
  /// <summary>
  /// Retrieves a phone number by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The phone number or null if not found.</returns>
  Task<Phone?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a phone number by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The phone number or null if not found.</returns>
  Task<Phone?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves the default phone number of the specified user.
  /// </summary>
  /// <param name="id">The identifier of the user to retrieve the default phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The phone number or null if not found.</returns>
  Task<Phone?> GetDefaultAsync(Guid userId, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a list of phone numbers using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="isArchived">The value filtering phone numbers on their archivation status.</param>
  /// <param name="isVerified">The value filtering phone numbers on their verification status.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="userId">The identifier of the user to filter by.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of phone numbers to skip.</param>
  /// <param name="take">The number of phone numbers to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of phone numbers, or empty if none found.</returns>
  Task<PagedList<Phone>> GetAsync(bool? isArchived = null, bool? isVerified = null, string? search = null, Guid? userId = null,
    PhoneSort? sort = null, bool isDescending = false, int? skip = null, int? take = null, CancellationToken cancellationToken = default);
}
