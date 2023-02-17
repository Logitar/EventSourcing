using Logitar.EventSourcing;
using Logitar.Identity.Realms;
using Logitar.Identity.Roles;

namespace Logitar.Identity.Users;

/// <summary>
/// Implements methods to help managing users.
/// </summary>
internal class UserHelper : IUserHelper
{
  /// <summary>
  /// The event store.
  /// </summary>
  private readonly IEventStore _eventStore;

  /// <summary>
  /// Initializes a new instance of the <see cref="UserHelper"/> class using the specified roles.
  /// </summary>
  /// <param name="eventStore">The event store.</param>
  public UserHelper(IEventStore eventStore)
  {
    _eventStore = eventStore;
  }

  /// <summary>
  /// Resolves a list of roles using the specified arguments.
  /// </summary>
  /// <param name="realm">The realm that the roles should belong to.</param>
  /// <param name="input">The user creation input data.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of the roles.</returns>
  public async Task<IEnumerable<RoleAggregate>?> GetRolesAsync(RealmAggregate realm,
    CreateUserInput input,
    CancellationToken cancellationToken = default)
  {
    return await GetRolesAsync(realm, input.Roles, nameof(input.Roles), cancellationToken);
  }

  /// <summary>
  /// Resolves a list of roles using the specified arguments.
  /// </summary>
  /// <param name="realm">The realm that the roles should belong to.</param>
  /// <param name="input">The user update input data.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of the roles.</returns>
  public async Task<IEnumerable<RoleAggregate>?> GetRolesAsync(RealmAggregate realm,
    UpdateUserInput input,
    CancellationToken cancellationToken = default)
  {
    return await GetRolesAsync(realm, input.Roles, nameof(input.Roles), cancellationToken);
  }

  /// <summary>
  /// Resolves a list of roles using the specified arguments.
  /// </summary>
  /// <param name="realm">The realm that the roles should belong to.</param>
  /// <param name="ids">The list of role identifiers.</param>
  /// <param name="paramName">The name of the role list parameter.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of the roles.</returns>
  /// <exception cref="AggregatesNotFoundException{RoleAggregate}">At least one role could not be found.</exception>
  /// <exception cref="RolesNotInRealmException">At least one role did not belong to the specified realm.</exception>
  private async Task<IEnumerable<RoleAggregate>?> GetRolesAsync(RealmAggregate realm,
    IEnumerable<Guid>? ids,
    string paramName,
    CancellationToken cancellationToken)
  {
    if (ids == null)
    {
      return null;
    }

    IEnumerable<AggregateId> aggregateIds = ids.Select(id => new AggregateId(id)).Distinct();
    List<AggregateId> missingRoles = new(capacity: aggregateIds.Count());
    List<RoleAggregate> notInRealm = new(capacity: missingRoles.Count);

    Dictionary<AggregateId, RoleAggregate> roles = (await _eventStore.LoadAsync<RoleAggregate>(aggregateIds, cancellationToken))
      .ToDictionary(x => x.Id, x => x);
    foreach (AggregateId id in aggregateIds)
    {
      if (!roles.TryGetValue(id, out RoleAggregate? role))
      {
        missingRoles.Add(id);
      }
      else if (role.RealmId != realm.Id)
      {
        notInRealm.Add(role);
      }
    }

    if (missingRoles.Any())
    {
      throw new AggregatesNotFoundException<RoleAggregate>(missingRoles, paramName);
    }
    else if (notInRealm.Any())
    {
      throw new RolesNotInRealmException(notInRealm, realm, paramName);
    }

    return roles.Values;
  }
}
