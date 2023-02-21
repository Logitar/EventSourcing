using Logitar.EventSourcing;

namespace Logitar.Identity.Contacts;

/// <summary>
/// Exposes methods to load email addresses from the event store.
/// </summary>
public interface IEmailRepository
{
  /// <summary>
  /// Retrieves the list of email addresses of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the email addresses.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of email addresses, or empty if none.</returns>
  Task<IEnumerable<EmailAggregate>> LoadByUserAsync(AggregateId userId, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves the default email address of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the default email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The default email address or null if not found.</returns>
  Task<EmailAggregate?> LoadDefaultAsync(AggregateId userId, CancellationToken cancellationToken = default);
}
