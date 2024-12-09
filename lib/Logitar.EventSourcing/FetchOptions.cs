namespace Logitar.EventSourcing;

public record FetchOptions
{
  public long FromVersion { get; set; }
  public long ToVersion { get; set; }
}
