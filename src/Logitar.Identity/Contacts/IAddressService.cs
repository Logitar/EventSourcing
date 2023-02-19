namespace Logitar.Identity.Contacts;

/// <summary>
/// Exposes methods to manage postal addresses in the identity system.
/// </summary>
public interface IAddressService
{
  /// <summary>
  /// Creates a new postal address.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created postal address.</returns>
  Task<Address> CreateAsync(CreateAddressInput input, CancellationToken cancellationToken = default);
  /// <summary>
  /// Deletes a postal address.
  /// </summary>
  /// <param name="id">The identifier of the postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted postal address.</returns>
  Task<Address> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a postal address by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the postal address.</param>
  /// <param name="userId">The identifier of the user's to get the default postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The postal address, or null if not found.</returns>
  Task<Address?> GetAsync(Guid? id = null, Guid? userId = null, CancellationToken cancellationToken = default);
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
  /// <summary>
  /// Sets a postal address the default for its user.
  /// </summary>
  /// <param name="id">The identifier of the postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The new default postal address.</returns>
  Task<Address> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Updates a postal address.
  /// </summary>
  /// <param name="id">The identifier of the postal address.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated postal address.</returns>
  Task<Address> UpdateAsync(Guid id, UpdateAddressInput input, CancellationToken cancellationToken = default);
  /// <summary>
  /// Verifies a postal address.
  /// </summary>
  /// <param name="id">The identifier of the postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The verified postal address.</returns>
  Task<Address> VerifyAsync(Guid id, CancellationToken cancellationToken = default);
}
