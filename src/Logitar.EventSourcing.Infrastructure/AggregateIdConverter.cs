namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a JSON converter for instances of <see cref="AggregateId"/> structs.
/// </summary>
public class AggregateIdConverter : JsonConverter<AggregateId>
{
  /// <summary>
  /// Reads an <see cref="AggregateId"/> from the specified JSON reader.
  /// </summary>
  /// <param name="reader">The JSON reader.</param>
  /// <param name="typeToConvert">The type to convert to.</param>
  /// <param name="options">The serializer options.</param>
  /// <returns>The resulting aggregate identifier.</returns>
  public override AggregateId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? default : new AggregateId(value);
  }

  /// <summary>
  /// Writes an <see cref="AggregateId"/> to the specified JSON writer.
  /// </summary>
  /// <param name="writer">The JSON writer.</param>
  /// <param name="aggregateId">The aggregate identifier to write.</param>
  /// <param name="options">The serializer options.</param>
  public override void Write(Utf8JsonWriter writer, AggregateId aggregateId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(aggregateId.Value);
  }
}
