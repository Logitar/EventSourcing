namespace Logitar.Identity;

/// <summary>
/// Provides extension methods for custom attributes.
/// </summary>
public static class CustomAttributesExtensions
{
  /// <summary>
  /// Converts the specified list of custom attributes to a dictionary.
  /// </summary>
  /// <param name="customAttributes">The list of custom attributes.</param>
  /// <returns>The dictionary of custom attributes.</returns>
  public static Dictionary<string, string> ToDictionary(this IEnumerable<CustomAttribute> customAttributes)
  {
    Dictionary<string, string> dictionary = new(capacity: customAttributes.Count());
    foreach (CustomAttribute customAttribute in customAttributes)
    {
      dictionary[customAttribute.Key] = customAttribute.Value;
    }

    return dictionary;
  }
}
