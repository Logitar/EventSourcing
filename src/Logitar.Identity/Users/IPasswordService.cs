using FluentValidation;
using Logitar.Identity.Realms;

namespace Logitar.Identity.Users;

/// <summary>
/// Exposes methods to manages passwords.
/// </summary>
internal interface IPasswordService
{
  /// <summary>
  /// Validates the specified password in the specified realm. If the password matches the realm
  /// password constraints, a salted and hashed password will be returned. If the password does not
  /// match the realm password constraints, a <see cref="ValidationException"/> will be thrown.
  /// </summary>
  /// <param name="realm">The realm defining password constraints.</param>
  /// <param name="password">The password to validate, then salt and hash.</param>
  /// <returns>The salted and hashed password.</returns>
  string ValidateAndHash(RealmAggregate realm, string password);
}
