using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

internal class FakeEventConverter : EventConverter
{
  private readonly Dictionary<string, Type> _eventTypeMap = new()
  {
    [typeof(UserCreated).Name] = typeof(UserCreated)
  };

  public FakeEventConverter(IEventSerializer serializer) : base(serializer)
  {
  }

  public Type GetEventTypeExposed(EventRecord record, EventMetadata? metadata) => GetEventType(record, metadata);
  protected override Type GetEventType(EventRecord record, EventMetadata? metadata)
  {
    return metadata == null && _eventTypeMap.TryGetValue(record.EventType, out Type? type)
      ? type
      : base.GetEventType(record, metadata);
  }

  public new T? GetEventMetadata<T>(EventRecord record) => base.GetEventMetadata<T>(record);
}
