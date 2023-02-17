using Logitar.Identity.Users.Commands;
using Logitar.Identity.Users.Queries;

namespace Logitar.Identity.Users;

/// <summary>
/// Implements methods to manage users in the identity system.
/// </summary>
internal class UserService : IUserService
{
  /// <summary>
  /// The request pipeline.
  /// </summary>
  private readonly IRequestPipeline _requestPipeline;

  /// <summary>
  /// Initializes a new instance of the <see cref="UserService"/> class using the specified arguments.
  /// </summary>
  /// <param name="requestPipeline">The request pipeline.</param>
  public UserService(IRequestPipeline requestPipeline)
  {
    _requestPipeline = requestPipeline;
  }

  /// <summary>
  /// Creates a new user.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created user.</returns>
  public async Task<User> CreateAsync(CreateUserInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new CreateUserCommand(input), cancellationToken);
  }

  /// <summary>
  /// Deletes an user.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted user.</returns>
  public async Task<User> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new DeleteUserCommand(id), cancellationToken);
  }

  /// <summary>
  /// Disables an user.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The disabled user.</returns>
  public async Task<User> DisableAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new DisableUserCommand(id), cancellationToken);
  }

  /// <summary>
  /// Enables an user.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The enabled user.</returns>
  public async Task<User> EnableAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new EnableUserCommand(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves an user by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="realm">The identifier or unique name of the realm in which to search the unique name.</param>
  /// <param name="username">The unique name of the user.</param>
  /// /// <param name="externalKey">The key of an external identifier.</param>
  /// <param name="externalValue">The value of an external identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user, or null if not found.</returns>
  public async Task<User?> GetAsync(Guid? id, string? realm, string? username,
    string? externalKey, string? externalValue, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetUserQuery(id, realm, username,
      externalKey, externalValue), cancellationToken);
  }

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
  public async Task<PagedList<User>> GetAsync(bool? isDisabled, string? realm, string? search,
    UserSort? sort, bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetUsersQuery(isDisabled, realm, search,
      sort, isDescending, skip, take), cancellationToken);
  }

  /// <summary>
  /// Adds, removes or updates the external identifier of an user.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="key">The key of the external identifier.</param>
  /// <param name="value">The value of the external identifier. If null, the external identifier will be removed.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated user.</returns>
  public async Task<User> SaveExternalIdentifierAsync(Guid id, string key, string? value, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new SaveExternalIdentifierCommand(id, key, value), cancellationToken);
  }

  /// <summary>
  /// Updates an user.
  /// </summary>
  /// <param name="id">The identifier of the user.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated user.</returns>
  public async Task<User> UpdateAsync(Guid id, UpdateUserInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new UpdateUserCommand(id, input), cancellationToken);
  }
}
