using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.JsonConverters;

/// <summary>
/// The converter used to serialize and deserialize instance of the <see cref="AggregateId"/> struct.
/// </summary>
public class AggregateIdConverter : JsonConverter<AggregateId>
{
  /// <summary>
  /// Reads an instance of the <see cref="AggregateId"/> struct from the specified JSON reader.
  /// </summary>
  /// <param name="reader">The JSON reader</param>
  /// <param name="typeToConvert">The type of the instance; it should be <see cref="AggregateId"/></param>
  /// <param name="options">The serialization options</param>
  /// <returns>The instance of the <see cref="AggregateId"/> struct</returns>
  public override AggregateId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? default : new AggregateId(value);
  }

  /// <summary>
  /// Writes an instance of the <see cref="AggregateId"/> struct to the specified JSON writer.
  /// </summary>
  /// <param name="writer">The JSON writer</param>
  /// <param name="value">The type of the instance; it should be <see cref="AggregateId"/</param>
  /// <param name="options">The serialization options</param>
  public override void Write(Utf8JsonWriter writer, AggregateId value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.Value);
  }
}
