using EventStore.Client;

namespace Logitar.EventSourcing.Kurrent;

public interface IEventConverter
{
  EventData ToEventData(IEvent @event, Type? streamType = null);
}
