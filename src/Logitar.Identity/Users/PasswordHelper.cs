using FluentValidation;
using Logitar.Identity.Realms;
using Logitar.Identity.Users.Validators;

namespace Logitar.Identity.Users;

/// <summary>
/// Implements methods to manages passwords.
/// </summary>
internal class PasswordHelper : IPasswordHelper
{
  /// <summary>
  /// Validates the specified password in the specified realm. If the password matches the realm
  /// password constraints, a salted and hashed password will be returned. If the password does not
  /// match the realm password constraints, a <see cref="ValidationException"/> will be thrown.
  /// </summary>
  /// <param name="realm">The realm defining password constraints.</param>
  /// <param name="password">The password to validate, then salt and hash.</param>
  /// <returns>The salted and hashed password.</returns>
  public string ValidateAndHash(RealmAggregate realm, string password)
  {
    new PasswordValidator(realm.PasswordSettings).ValidateAndThrow(password);

    return new Pbkdf2(password).ToString();
  }
}
