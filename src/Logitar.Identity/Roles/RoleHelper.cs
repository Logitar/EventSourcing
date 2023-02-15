namespace Logitar.Identity.Roles;

/// <summary>
/// Provides methods to help managing roles.
/// </summary>
internal static class RoleHelper
{
  /// <summary>
  /// Converts the specified list of custom attributes to a dictionary of custom attributes.
  /// </summary>
  /// <param name="customAttributes">The list of custom attributes.</param>
  /// <returns>The dictionary of custom attributes.</returns>
  public static Dictionary<string, string>? GetCustomAttributes(IEnumerable<CustomAttribute>? customAttributes)
  {
    if (customAttributes == null)
    {
      return null;
    }

    Dictionary<string, string> dictionary = new(capacity: customAttributes.Count());
    foreach (CustomAttribute customAttribute in customAttributes)
    {
      dictionary[customAttribute.Key] = customAttribute.Value;
    }

    return dictionary;
  }
}
