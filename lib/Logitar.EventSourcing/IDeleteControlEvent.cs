namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event that can control the deletion status of its stream.
/// </summary>
public interface IDeleteControlEvent : IEvent
{
  /// <summary>
  /// Gets a value indicating whether or not the stream is deleted.
  /// </summary>
  bool? IsDeleted { get; }
}
