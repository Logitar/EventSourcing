using System.Globalization;

namespace Logitar.Identity;

/// <summary>
/// Provides extension methods for the <see cref="string"/> class.
/// </summary>
public static class StringExtensions
{
  /// <summary>
  /// Retrieves a cached, read-only instance of a culture using the specified culture name.
  /// </summary>
  /// <param name="name">The name of the culture.</param>
  /// <returns>The culture instance, or null if the name was null.</returns>
  public static CultureInfo? GetCultureInfo(this string? name) => name == null ? null : CultureInfo.GetCultureInfo(name);
}
