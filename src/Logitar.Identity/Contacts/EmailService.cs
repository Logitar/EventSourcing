using Logitar.Identity.Contacts.Commands;
using Logitar.Identity.Contacts.Queries;

namespace Logitar.Identity.Contacts;

/// <summary>
/// Implements methods to manage email addresses in the identity system.
/// </summary>
internal class EmailService : IEmailService
{
  /// <summary>
  /// The request pipeline.
  /// </summary>
  private readonly IRequestPipeline _requestPipeline;

  /// <summary>
  /// Initializes a new instance of the <see cref="EmailService"/> class using the specified arguments.
  /// </summary>
  /// <param name="requestPipeline">The request pipeline.</param>
  public EmailService(IRequestPipeline requestPipeline)
  {
    _requestPipeline = requestPipeline;
  }

  /// <summary>
  /// Creates a new email address.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created email address.</returns>
  public async Task<Email> CreateAsync(CreateEmailInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new CreateEmailCommand(input), cancellationToken);
  }

  /// <summary>
  /// Deletes an email address.
  /// </summary>
  /// <param name="id">The identifier of the email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted email address.</returns>
  public async Task<Email> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new DeleteEmailCommand(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves an email address by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the email address.</param>
  /// <param name="userId">The identifier of the user to get the default email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The email address, or null if not found.</returns>
  public async Task<Email?> GetAsync(Guid? id, Guid? userId, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetEmailQuery(id, userId), cancellationToken);
  }

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
  public async Task<PagedList<Email>> GetAsync(bool? isArchived, bool? isVerified, string? search, Guid? userId,
    EmailSort? sort, bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetEmailsQuery(isArchived, isVerified, search, userId,
      sort, isDescending, skip, take), cancellationToken);
  }

  /// <summary>
  /// Sets an email address the default for its user.
  /// </summary>
  /// <param name="id">The identifier of the email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The new default email address.</returns>
  public async Task<Email> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new SetDefaultEmailCommand(id), cancellationToken);
  }

  /// <summary>
  /// Updates an email address.
  /// </summary>
  /// <param name="id">The identifier of the email address.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated email address.</returns>
  public async Task<Email> UpdateAsync(Guid id, UpdateEmailInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new UpdateEmailCommand(id, input), cancellationToken);
  }

  /// <summary>
  /// Verifies an email address.
  /// </summary>
  /// <param name="id">The identifier of the email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The verified email address.</returns>
  public async Task<Email> VerifyAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new VerifyEmailCommand(id), cancellationToken);
  }
}
