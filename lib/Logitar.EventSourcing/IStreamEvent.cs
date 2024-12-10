namespace Logitar.EventSourcing;

public interface IStreamEvent : IEvent
{
  StreamId StreamId { get; }
}
