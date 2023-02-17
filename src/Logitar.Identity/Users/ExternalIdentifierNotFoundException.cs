using System.Text;

namespace Logitar.Identity.Users;

/// <summary>
/// The exception thrown when an external identifier could not be found.
/// </summary>
public class ExternalIdentifierNotFoundException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ExternalIdentifierNotFoundException"/> class using the specified arguments.
  /// </summary>
  /// <param name="user">The user that has been searched.</param>
  /// <param name="key">The key of the external identifier.</param>
  public ExternalIdentifierNotFoundException(UserAggregate user, string key) : base(GetMessage(user, key))
  {
    Data["User"] = user.ToString();
    Data["Key"] = key;
  }

  /// <summary>
  /// Builds the exception message using the specified arguments.
  /// </summary>
  /// <param name="user">The user that has been searched.</param>
  /// <param name="key">The key of the external identifier.</param>
  /// <returns>The exception message.</returns>
  private static string GetMessage(UserAggregate user, string key)
  {
    StringBuilder message = new();

    message.AppendLine("The specified external identifier was not found for the specified user.");
    message.AppendLine($"User: {user}");
    message.AppendLine($"Key: {key}");

    return message.ToString();
  }
}
