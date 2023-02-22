namespace Logitar.EventSourcing;

/// <summary>
/// Provides extension methods for the <see cref="string"/> class.
/// </summary>
public static class StringExtensions
{
  /// <summary>
  /// Returns null if the specified string is null, empty or only white spaces; otherwise, returns
  /// the trimmed string.
  /// </summary>
  /// <param name="s">The string to clean.</param>
  /// <returns>The cleant string.</returns>
  public static string? CleanTrim(this string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

  /// <summary>
  /// Decodes a string from the uri-safe base64 encoding. Plus signs (+) are retrieved from hyphens (-),
  /// slashes (/) from underscores (_) and equal signs (=) are restored at the end of the string.
  /// </summary>
  /// <param name="s">The string to decode.</param>
  /// <returns>The decoded string.</returns>
  public static string FromUriSafeBase64(this string s)
  {
    if (s.Length % 4 > 0)
    {
      s = s.PadRight((s.Length / 4 + 1) * 4, '=');
    }

    return s.Replace('_', '/').Replace('-', '+');
  }

  /// <summary>
  /// Removes all occurrences of the specified pattern in the specified string.
  /// </summary>
  /// <param name="s">The original string.</param>
  /// <param name="pattern">The pattern to remove.</param>
  /// <returns>The string with the pattern removed.</returns>
  public static string Remove(this string s, string pattern) => s.Replace(pattern, null);

  /// <summary>
  /// Encodes a string to the uri-safe base64 encoding. Plus signs (+) are encoded as hyphens (-),
  /// slashes (/) as underscores (_) and equal signs (=) at the end of the string are removed.
  /// </summary>
  /// <param name="s">The string to encode.</param>
  /// <returns>The encoded string.</returns>
  public static string ToUriSafeBase64(this string s) => s.Replace('+', '_').Replace('/', '_').TrimEnd('=');
}
