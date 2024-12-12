namespace Logitar.EventSourcing;

/// <summary>
/// Represents options for fetching many streams of events.
/// </summary>
public record FetchManyOptions : FetchOptions
{
  /// <summary>
  /// Gets or sets the list of stream types to filter by. If no type is specified, this filter will be ignored.
  /// </summary>
  public List<Type?> StreamTypes { get; set; } = [];
}
