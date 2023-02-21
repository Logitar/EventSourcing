using Logitar.EventSourcing;

namespace Logitar.Identity.Contacts;

/// <summary>
/// Exposes methods to load postal addresses from the event store.
/// </summary>
public interface IAddressRepository
{
  /// <summary>
  /// Retrieves the list of postal addresses of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the postal addresses.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of postal addresses, or empty if none.</returns>
  Task<IEnumerable<AddressAggregate>> LoadByUserAsync(AggregateId userId, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves the default postal address of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the default postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The default postal address or null if not found.</returns>
  Task<AddressAggregate?> LoadDefaultAsync(AggregateId userId, CancellationToken cancellationToken = default);
}
