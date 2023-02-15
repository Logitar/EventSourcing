using System.Text;

namespace Logitar.Identity;

/// <summary>
/// The exception thrown when an unique name is already used.
/// </summary>
public class UniqueNameAlreadyUsedException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UniqueNameAlreadyUsedException"/> class using the specified arguments.
  /// </summary>
  /// <param name="uniqueName">The unique name that is already used.</param>
  /// <param name="paramName">The name of the parameter associated to the unique name.</param>
  public UniqueNameAlreadyUsedException(string uniqueName, string paramName) : base(GetMessage(uniqueName, paramName))
  {
    Data["UniqueName"] = uniqueName;
    Data["ParamName"] = paramName;
  }

  /// <summary>
  /// Builds the exception message using the specified arguments.
  /// </summary>
  /// <param name="uniqueName">The unique name that is already used.</param>
  /// <param name="paramName">The name of the parameter associated to the unique name.</param>
  /// <returns>The exception message.</returns>
  private static string GetMessage(string uniqueName, string paramName)
  {
    StringBuilder message = new();

    message.AppendLine("The specified unique name is already used.");
    message.AppendLine($"Unique name: {uniqueName}");
    message.AppendLine($"ParamName: {paramName}");

    return message.ToString();
  }
}
