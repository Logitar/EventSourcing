using Logitar.Identity.Realms;
using System.Text;

namespace Logitar.Identity.Roles;

/// <summary>
/// The exception thrown when roles does not belong to a realm.
/// </summary>
public class RolesNotInRealmException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RolesNotInRealmException"/> class using the specified arguments.
  /// </summary>
  /// <param name="roles">The list of roles not in the realm.</param>
  /// <param name="realm">The realm the roles should belong to.</param>
  public RolesNotInRealmException(IEnumerable<RoleAggregate> roles, RealmAggregate realm) : base(GetMessage(roles, realm))
  {
    Data["Roles"] = roles.Select(role => role.ToString());
    Data["Realm"] = realm.ToString();
  }

  /// <summary>
  /// Builds the exception message using the specified arguments.
  /// </summary>
  /// <param name="roles">The list of roles not in the realm.</param>
  /// <param name="realm">The realm the roles should belong to.</param>
  /// <returns>The exception message.</returns>
  private static string GetMessage(IEnumerable<RoleAggregate> roles, RealmAggregate realm)
  {
    StringBuilder message = new();

    message.AppendLine("The specified roles does not belong to the specified realm.");
    message.AppendLine($"Realm: {realm}");
    message.AppendLine("Roles:");

    foreach (RoleAggregate role in roles)
    {
      message.AppendLine($" - {role}");
    }

    return message.ToString();
  }
}
