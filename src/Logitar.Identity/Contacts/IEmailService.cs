namespace Logitar.Identity.Contacts;

/// <summary>
/// Exposes methods to manage email addresses in the identity system.
/// </summary>
public interface IEmailService
{
  /// <summary>
  /// Creates a new email address.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created email address.</returns>
  Task<Email> CreateAsync(CreateEmailInput input, CancellationToken cancellationToken = default);
  /// <summary>
  /// Deletes a email address.
  /// </summary>
  /// <param name="id">The identifier of the email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted email address.</returns>
  Task<Email> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a email address by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the email address.</param>
  /// <param name="userId">The identifier of the user's to get the default email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The email address, or null if not found.</returns>
  Task<Email?> GetAsync(Guid? id = null, Guid? userId = null, CancellationToken cancellationToken = default);
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
  /// <summary>
  /// Sets a email address the default for its user.
  /// </summary>
  /// <param name="id">The identifier of the email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The new default email address.</returns>
  Task<Email> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Updates a email address.
  /// </summary>
  /// <param name="id">The identifier of the email address.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated email address.</returns>
  Task<Email> UpdateAsync(Guid id, UpdateEmailInput input, CancellationToken cancellationToken = default);
  /// <summary>
  /// Verifies a email address.
  /// </summary>
  /// <param name="id">The identifier of the email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The verified email address.</returns>
  Task<Email> VerifyAsync(Guid id, CancellationToken cancellationToken = default);
}
