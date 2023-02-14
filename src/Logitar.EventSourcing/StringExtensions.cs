namespace Logitar.EventSourcing;

public static class StringExtensions
{
  public static string? CleanTrim(this string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

  public static string FromUriSafeBase64(this string s)
  {
    if (s.Length % 4 > 0)
    {
      s = s.PadRight((s.Length / 4 + 1) * 4, '=');
    }

    return s.Replace('_', '/').Replace('-', '+');
  }

  public static string Remove(this string s, string pattern) => s.Replace(pattern, null);

  public static string ToUriSafeBase64(this string s) => s.Replace('+', '_').Replace('/', '_').TrimEnd('=');
}
