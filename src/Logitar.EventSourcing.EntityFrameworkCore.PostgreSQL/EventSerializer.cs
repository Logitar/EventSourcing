using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.JsonConverters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

public class EventSerializer
{
  private static EventSerializer? _instance = null;
  public static EventSerializer Instance
  {
    get
    {
      _instance ??= new();
      return _instance;
    }
  }

  private readonly JsonSerializerOptions _options = new();

  private EventSerializer()
  {
    RegisterConverter(new AggregateIdConverter());
    RegisterConverter(new CultureInfoConverter());
  }

  public void RegisterConverter(JsonConverter converter)
  {
    _options.Converters.Add(converter);
  }

  public DomainEvent Deserialize(EventEntity entity)
  {
    Type? eventType = Type.GetType(entity.EventType);
    if (eventType == null)
    {
      StringBuilder message = new();
      message.AppendLine("The specified event type could not be resolved.");
      message.AppendLine($"Event: EventId={entity.EventId}, Id={entity.Id}");
      message.AppendLine($"EventType: {entity.EventType}");
      throw new InvalidOperationException(message.ToString());
    }

    DomainEvent? change = (DomainEvent?)JsonSerializer.Deserialize(entity.EventData, eventType, _options);
    if (change == null)
    {
      StringBuilder message = new();
      message.AppendLine("The specified event data could not be deserialized.");
      message.AppendLine($"Event: EventId={entity.EventId}, Id={entity.Id}");
      message.AppendLine($"EventData: {entity.EventData}");
      throw new InvalidOperationException(message.ToString());
    }

    return change;
  }

  public string Serialize(DomainEvent change)
  {
    return JsonSerializer.Serialize(change, change.GetType(), _options);
  }
}
