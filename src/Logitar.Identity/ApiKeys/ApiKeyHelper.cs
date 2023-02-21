using Logitar.EventSourcing;
using Logitar.Identity.Realms;
using Logitar.Identity.Roles;
using Logitar.Identity.Users;
using System.Security.Cryptography;

namespace Logitar.Identity.ApiKeys;

/// <summary>
/// Implements methods to help managing API keys.
/// </summary>
internal class ApiKeyHelper : IApiKeyHelper
{
  /// <summary>
  /// The event store.
  /// </summary>
  private readonly IEventStore _eventStore;

  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyHelper"/> class using the specified roles.
  /// </summary>
  /// <param name="eventStore">The event store.</param>
  public ApiKeyHelper(IEventStore eventStore)
  {
    _eventStore = eventStore;
  }

  /// <summary>
  /// Generates an API key secret.
  /// </summary>
  /// <param name="secret">The secret bytes.</param>
  /// <returns>The salted and hashed secret.</returns>
  public string GenerateSecret(out byte[] secret)
  {
    secret = RandomNumberGenerator.GetBytes(32);

    return new Pbkdf2(Convert.ToBase64String(secret)).ToString();
  }

  /// <summary>
  /// Resolves a list of roles using the specified arguments.
  /// </summary>
  /// <param name="realm">The realm that the roles should belong to.</param>
  /// <param name="input">The API key creation input data.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of the roles.</returns>
  public async Task<IEnumerable<RoleAggregate>?> GetRolesAsync(RealmAggregate realm,
    CreateApiKeyInput input,
    CancellationToken cancellationToken = default)
  {
    return await GetRolesAsync(realm, input.Roles, nameof(input.Roles), cancellationToken);
  }

  /// <summary>
  /// Resolves a list of roles using the specified arguments.
  /// </summary>
  /// <param name="realm">The realm that the roles should belong to.</param>
  /// <param name="input">The API key update input data.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of the roles.</returns>
  public async Task<IEnumerable<RoleAggregate>?> GetRolesAsync(RealmAggregate realm,
    UpdateApiKeyInput input,
    CancellationToken cancellationToken = default)
  {
    return await GetRolesAsync(realm, input.Roles, nameof(input.Roles), cancellationToken);
  }

  /// <summary>
  /// Returns a value indicating whether or not the specified secret matches the specified API key.
  /// </summary>
  /// <param name="apiKey">The API key to compare.</param>
  /// <param name="secret">The secret to match.</param>
  /// <returns>True if the secret matches the API key's salted and hashed secret.</returns>
  public bool IsMatch(ApiKeyAggregate apiKey, byte[] secret)
  {
    Pbkdf2 pbkdf2 = Pbkdf2.Parse(apiKey.SecretHash);

    return pbkdf2.IsMatch(Convert.ToBase64String(secret));
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
