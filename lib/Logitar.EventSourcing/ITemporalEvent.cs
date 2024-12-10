namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event that has occurred at some time in the past.
/// </summary>
public interface ITemporalEvent : IEvent
{
  /// <summary>
  /// Gets the date and time when the event occurred.
  /// </summary>
  DateTime OccurredOn { get; }
}
