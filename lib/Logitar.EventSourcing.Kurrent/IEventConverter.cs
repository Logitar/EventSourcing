using EventStore.Client;

namespace Logitar.EventSourcing.Kurrent;

public interface IEventConverter
{
  Type? GetStreamType(EventRecord record);
  Event ToEvent(EventRecord record);

  EventData ToEventData(IEvent @event, Type? streamType = null);
}
