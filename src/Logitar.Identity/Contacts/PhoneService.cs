using Logitar.Identity.Contacts.Commands;
using Logitar.Identity.Contacts.Queries;

namespace Logitar.Identity.Contacts;

/// <summary>
/// Implements methods to manage phone numbers in the identity system.
/// </summary>
internal class PhoneService : IPhoneService
{
  /// <summary>
  /// The request pipeline.
  /// </summary>
  private readonly IRequestPipeline _requestPipeline;

  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneService"/> class using the specified arguments.
  /// </summary>
  /// <param name="requestPipeline">The request pipeline.</param>
  public PhoneService(IRequestPipeline requestPipeline)
  {
    _requestPipeline = requestPipeline;
  }

  /// <summary>
  /// Creates a new phone number.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created phone number.</returns>
  public async Task<Phone> CreateAsync(CreatePhoneInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new CreatePhoneCommand(input), cancellationToken);
  }

  /// <summary>
  /// Deletes a phone number.
  /// </summary>
  /// <param name="id">The identifier of the phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted phone number.</returns>
  public async Task<Phone> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new DeletePhoneCommand(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves a phone number by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the phone number.</param>
  /// <param name="userId">The identifier of the user to get the default phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The phone number, or null if not found.</returns>
  public async Task<Phone?> GetAsync(Guid? id, Guid? userId, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetPhoneQuery(id, userId), cancellationToken);
  }

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
  public async Task<PagedList<Phone>> GetAsync(bool? isArchived, bool? isVerified, string? search, Guid? userId,
    PhoneSort? sort, bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetPhonesQuery(isArchived, isVerified, search, userId,
      sort, isDescending, skip, take), cancellationToken);
  }

  /// <summary>
  /// Sets a phone number the default for its user.
  /// </summary>
  /// <param name="id">The identifier of the phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The new default phone number.</returns>
  public async Task<Phone> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new SetDefaultPhoneCommand(id), cancellationToken);
  }

  /// <summary>
  /// Updates a phone number.
  /// </summary>
  /// <param name="id">The identifier of the phone number.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated phone number.</returns>
  public async Task<Phone> UpdateAsync(Guid id, UpdatePhoneInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new UpdatePhoneCommand(id, input), cancellationToken);
  }

  /// <summary>
  /// Verifies a phone number.
  /// </summary>
  /// <param name="id">The identifier of the phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The verified phone number.</returns>
  public async Task<Phone> VerifyAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new VerifyPhoneCommand(id), cancellationToken);
  }
}
