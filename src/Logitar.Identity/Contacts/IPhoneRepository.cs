using Logitar.EventSourcing;

namespace Logitar.Identity.Contacts;

/// <summary>
/// Exposes methods to load phone numbers from the event store.
/// </summary>
public interface IPhoneRepository
{
  /// <summary>
  /// Retrieves the list of phone numbers of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the phone numbers.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of phone numbers, or empty if none.</returns>
  Task<IEnumerable<PhoneAggregate>> LoadByUserAsync(AggregateId userId, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves the default phone number of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the default phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The phone numbers or null if not found.</returns>
  Task<PhoneAggregate?> LoadDefaultAsync(AggregateId userId, CancellationToken cancellationToken = default);
}
