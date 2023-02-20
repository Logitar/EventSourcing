using Logitar.Identity.Contacts.Commands;
using Logitar.Identity.Contacts.Queries;

namespace Logitar.Identity.Contacts;

/// <summary>
/// Implements methods to manage postal addresses in the identity system.
/// </summary>
internal class AddressService : IAddressService
{
  /// <summary>
  /// The request pipeline.
  /// </summary>
  private readonly IRequestPipeline _requestPipeline;

  /// <summary>
  /// Initializes a new instance of the <see cref="AddressService"/> class using the specified arguments.
  /// </summary>
  /// <param name="requestPipeline">The request pipeline.</param>
  public AddressService(IRequestPipeline requestPipeline)
  {
    _requestPipeline = requestPipeline;
  }

  /// <summary>
  /// Creates a new postal address.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created postal address.</returns>
  public async Task<Address> CreateAsync(CreateAddressInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new CreateAddressCommand(input), cancellationToken);
  }

  /// <summary>
  /// Deletes a postal address.
  /// </summary>
  /// <param name="id">The identifier of the postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted postal address.</returns>
  public async Task<Address> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new DeleteAddressCommand(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves a postal address by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the postal address.</param>
  /// <param name="userId">The identifier of the user to get the default postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The postal address, or null if not found.</returns>
  public async Task<Address?> GetAsync(Guid? id, Guid? userId, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetAddressQuery(id, userId), cancellationToken);
  }

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
  public async Task<PagedList<Address>> GetAsync(bool? isArchived, bool? isVerified, string? search, Guid? userId,
    AddressSort? sort, bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetAddressesQuery(isArchived, isVerified, search, userId,
      sort, isDescending, skip, take), cancellationToken);
  }

  /// <summary>
  /// Sets a postal address the default for its user.
  /// </summary>
  /// <param name="id">The identifier of the postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The new default postal address.</returns>
  public async Task<Address> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new SetDefaultAddressCommand(id), cancellationToken);
  }

  /// <summary>
  /// Updates a postal address.
  /// </summary>
  /// <param name="id">The identifier of the postal address.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated postal address.</returns>
  public async Task<Address> UpdateAsync(Guid id, UpdateAddressInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new UpdateAddressCommand(id, input), cancellationToken);
  }

  /// <summary>
  /// Verifies a postal address.
  /// </summary>
  /// <param name="id">The identifier of the postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The verified postal address.</returns>
  public async Task<Address> VerifyAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new VerifyAddressCommand(id), cancellationToken);
  }
}
