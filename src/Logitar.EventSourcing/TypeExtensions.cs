namespace Logitar.EventSourcing;

/// <summary>
/// Provides extension methods for the <see cref="Type"/> class.
/// </summary>
public static class TypeExtensions
{
  /// <summary>
  /// Returns the most precise name of a <see cref="Type"/>, starting with its AssemblyQualifiedName,
  /// then its FullName, and finally its Name.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns>The type's name.</returns>
  public static string GetName(this Type type) => type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
}
