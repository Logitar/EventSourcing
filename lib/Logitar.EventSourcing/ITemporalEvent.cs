namespace Logitar.EventSourcing;

/// <summary>
/// TODO(fpion): document
/// </summary>
public interface ITemporalEvent : IEvent
{
  /// <summary>
  /// TODO(fpion): document
  /// </summary>
  DateTime OccurredOn { get; }
}
