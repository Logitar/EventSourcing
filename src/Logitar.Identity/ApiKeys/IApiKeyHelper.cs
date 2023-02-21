using Logitar.Identity.Realms;
using Logitar.Identity.Roles;

namespace Logitar.Identity.ApiKeys;

/// <summary>
/// Defines methods to help managing API keys.
/// </summary>
internal interface IApiKeyHelper
{
  /// <summary>
  /// Generates an API key secret.
  /// </summary>
  /// <param name="secret">The secret bytes.</param>
  /// <returns>The salted and hashed secret.</returns>
  string GenerateSecret(out byte[] secret);

  /// <summary>
  /// Resolves a list of roles using the specified arguments.
  /// </summary>
  /// <param name="realm">The realm that the roles should belong to.</param>
  /// <param name="input">The API key creation input data.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of the roles.</returns>
  Task<IEnumerable<RoleAggregate>?> GetRolesAsync(RealmAggregate realm,
    CreateApiKeyInput input,
    CancellationToken cancellationToken = default);

  /// <summary>
  /// Resolves a list of roles using the specified arguments.
  /// </summary>
  /// <param name="realm">The realm that the roles should belong to.</param>
  /// <param name="input">The API key update input data.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of the roles.</returns>
  Task<IEnumerable<RoleAggregate>?> GetRolesAsync(RealmAggregate realm,
    UpdateApiKeyInput input,
    CancellationToken cancellationToken = default);

  /// <summary>
  /// Returns a value indicating whether or not the specified secret matches the specified API key.
  /// </summary>
  /// <param name="apiKey">The API key to compare.</param>
  /// <param name="secret">The secret to match.</param>
  /// <returns>True if the secret matches the API key's salted and hashed secret.</returns>
  bool IsMatch(ApiKeyAggregate apiKey, byte[] secret);
}
