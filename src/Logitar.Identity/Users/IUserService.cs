namespace Logitar.Identity.Users;

/// <summary>
/// Exposes methods to manage users in the identity system.
/// </summary>
public interface IUserService
{
  /// <summary>
  /// Creates a new user.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created user.</returns>
  Task<User> CreateAsync(CreateUserInput input, CancellationToken cancellationToken = default);
  /// <summary>
  /// Deletes an user.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted user.</returns>
  Task<User> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Disables an user.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The disabled user.</returns>
  Task<User> DisableAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Enables an user.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The enabled user.</returns>
  Task<User> EnableAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves an user by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="realm">The identifier or unique name of the realm in which to search the unique name.</param>
  /// <param name="username">The unique name of the user.</param>
  /// <param name="externalKey">The key of an external identifier.</param>
  /// <param name="externalValue">The value of an external identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user, or null if not found.</returns>
  Task<User?> GetAsync(Guid? id = null, string? realm = null, string? username = null,
    string? externalKey = null, string? externalValue = null, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a list of users using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="isDisabled">The value filtering users on their disabled status.</param>
  /// <param name="realm">The identifier or unique name of the realm to filter by.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of users to skip.</param>
  /// <param name="take">The number of users to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of users, or empty if none found.</returns>
  Task<PagedList<User>> GetAsync(bool? isDisabled = null, string? realm = null, string? search = null,
    UserSort? sort = null, bool isDescending = false, int? skip = null, int? take = null, CancellationToken cancellationToken = default);
  /// <summary>
  /// Adds, removes or updates the external identifier of an user.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="key">The key of the external identifier.</param>
  /// <param name="value">The value of the external identifier. If null, the external identifier will be removed.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated user.</returns>
  Task<User> SaveExternalIdentifierAsync(Guid id, string key, string? value, CancellationToken cancellationToken = default);
  /// <summary>
  /// Updates an user.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated user.</returns>
  Task<User> UpdateAsync(Guid id, UpdateUserInput input, CancellationToken cancellationToken = default);
}
