namespace Logitar.EventSourcing.Demo.Infrastructure.DemoDb;

internal static class Helper
{
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
