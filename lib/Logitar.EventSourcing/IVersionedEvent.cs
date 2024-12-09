namespace Logitar.EventSourcing;

/// <summary>
/// TODO(fpion): document
/// </summary>
public interface IVersionedEvent : IEvent
{
  /// <summary>
  /// TODO(fpion): document
  /// </summary>
  long Version { get; }
}
