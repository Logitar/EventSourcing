namespace Logitar.Identity.Contacts;

/// <summary>
/// Exposes methods to manage phone numbers in the identity system.
/// </summary>
public interface IPhoneService
{
  /// <summary>
  /// Creates a new phone number.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created phone number.</returns>
  Task<Phone> CreateAsync(CreatePhoneInput input, CancellationToken cancellationToken = default);
  /// <summary>
  /// Deletes a phone number.
  /// </summary>
  /// <param name="id">The identifier of the phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted phone number.</returns>
  Task<Phone> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a phone number by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the phone number.</param>
  /// <param name="userId">The identifier of the user's to get the default phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The phone number, or null if not found.</returns>
  Task<Phone?> GetAsync(Guid? id = null, Guid? userId = null, CancellationToken cancellationToken = default);
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
  /// <summary>
  /// Sets a phone number the default for its user.
  /// </summary>
  /// <param name="id">The identifier of the phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The new default phone number.</returns>
  Task<Phone> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Updates a phone number.
  /// </summary>
  /// <param name="id">The identifier of the phone number.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated phone number.</returns>
  Task<Phone> UpdateAsync(Guid id, UpdatePhoneInput input, CancellationToken cancellationToken = default);
  /// <summary>
  /// Verifies a phone number.
  /// </summary>
  /// <param name="id">The identifier of the phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The verified phone number.</returns>
  Task<Phone> VerifyAsync(Guid id, CancellationToken cancellationToken = default);
}
