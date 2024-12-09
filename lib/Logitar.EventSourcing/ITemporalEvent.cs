namespace Logitar.EventSourcing;

public interface ITemporalEvent : IEvent
{
  DateTime OccurredOn { get; }
}
