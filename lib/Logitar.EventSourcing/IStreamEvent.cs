namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event belonging to a stream.
/// </summary>
public interface IStreamEvent : IEvent
{
  /// <summary>
  /// Gets the identifier of the stream to which the event belongs to.
  /// </summary>
  StreamId StreamId { get; }
}
