using System.Text;

namespace Logitar.Identity.Users;

/// <summary>
/// The exception thrown when an external identifier already exists.
/// </summary>
public class ExternalIdentifierAlreadyExisting : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ExternalIdentifierAlreadyExisting"/> class using the specified key and value.
  /// </summary>
  /// <param name="key">The external identifier key.</param>
  /// <param name="value">The external identifier value.</param>
  public ExternalIdentifierAlreadyExisting(string key, string value) : base(GetMessage(key, value))
  {
    Data["Key"] = key;
    Data["Value"] = value;
  }

  /// <summary>
  /// Builds the exception message using the specified key and value.
  /// </summary>
  /// <param name="key">The external identifier key.</param>
  /// <param name="value">The external identifier value.</param>
  /// <returns>The exception message.</returns>
  private static string GetMessage(string key, string value)
  {
    StringBuilder message = new();

    message.AppendLine("The specified external identifier already exists.");
    message.AppendLine($"Key: {key}");
    message.AppendLine($"Value: {value}");

    return message.ToString();
  }
}
