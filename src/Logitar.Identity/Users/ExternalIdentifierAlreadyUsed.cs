using System.Text;

namespace Logitar.Identity.Users;

/// <summary>
/// The exception thrown when an external identifier is already used.
/// </summary>
public class ExternalIdentifierAlreadyUsed : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ExternalIdentifierAlreadyUsed"/> class using the specified key and value.
  /// </summary>
  /// <param name="user">The user owning the external identifier.</param>
  /// <param name="key">The external identifier key.</param>
  /// <param name="value">The external identifier value.</param>
  public ExternalIdentifierAlreadyUsed(UserAggregate user, string key, string value)
    : base(GetMessage(user, key, value))
  {
    Data["User"] = user.ToString();
    Data["Key"] = key;
    Data["Value"] = value;
  }

  /// <summary>
  /// Builds the exception message using the specified arguments.
  /// </summary>
  /// <param name="user">The user owning the external identifier.</param>
  /// <param name="key">The external identifier key.</param>
  /// <param name="value">The external identifier value.</param>
  /// <returns>The exception message.</returns>
  private static string GetMessage(UserAggregate user, string key, string value)
  {
    StringBuilder message = new();

    message.AppendLine("The specified external identifier is already used by the specified user.");
    message.AppendLine($"User: {user}");
    message.AppendLine($"Key: {key}");
    message.AppendLine($"Value: {value}");

    return message.ToString();
  }
}
