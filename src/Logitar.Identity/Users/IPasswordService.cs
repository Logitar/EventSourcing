using FluentValidation;
using Logitar.Identity.Realms;

namespace Logitar.Identity.Users;

/// <summary>
/// Exposes methods to manages passwords.
/// </summary>
internal interface IPasswordService
{
  /// <summary>
  /// Salts and hashes the specified password.
  /// </summary>
  /// <param name="password">The password to salt and hash.</param>
  /// <returns>The salted and hashed password.</returns>
  string Hash(string password);
  /// <summary>
  /// Validates the specified password in the specified realm. A <see cref="ValidationException"/>
  /// will be thrown if the password does not match the realm password constraints.
  /// </summary>
  /// <param name="realm">The realm defining password constraints.</param>
  /// <param name="password">The password to validate.</param>
  void ValidateAndThrow(RealmAggregate realm, string password);
}
