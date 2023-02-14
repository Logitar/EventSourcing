using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.JsonConverters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// The serializer used to serialize and deserialize domain events using the JSON format.
/// </summary>
public class EventSerializer
{
  /// <summary>
  /// The Singleton instance of the serializer
  /// </summary>
  private static EventSerializer? _instance = null;
  /// <summary>
  /// The Singleton instance of the serializer
  /// </summary>
  public static EventSerializer Instance
  {
    get
    {
      _instance ??= new();
      return _instance;
    }
  }

  /// <summary>
  /// The serialization options used
  /// </summary>
  private readonly JsonSerializerOptions _options = new();

  /// <summary>
  /// Initializes the static class <see cref="EventSerializer"/>.
  /// </summary>
  private EventSerializer()
  {
    RegisterConverter(new AggregateIdConverter());
    RegisterConverter(new CultureInfoConverter());
  }

  /// <summary>
  /// Registers the specified JSON converter to the serializer. Please note you cannot register a new converter after the serializer has been used once.
  /// </summary>
  /// <param name="converter">The converter to register</param>
  public void RegisterConverter(JsonConverter converter)
  {
    _options.Converters.Add(converter);
  }

  /// <summary>
  /// Deserializes a domain event from the specified event entity.
  /// </summary>
  /// <param name="entity">The event entity</param>
  /// <returns>The deserialized domain event</returns>
  /// <exception cref="InvalidOperationException">The event could not be resolved, or the event data could not be deserialized</exception>
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

  /// <summary>
  /// Serializes the specified domain event to JSON.
  /// </summary>
  /// <param name="change">The domain event to serialize</param>
  /// <returns>The JSON representation of the domain event</returns>
  public string Serialize(DomainEvent change)
  {
    return JsonSerializer.Serialize(change, change.GetType(), _options);
  }
}
