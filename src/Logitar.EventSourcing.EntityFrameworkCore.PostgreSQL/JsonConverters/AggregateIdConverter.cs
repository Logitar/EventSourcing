using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.JsonConverters;

public class AggregateIdConverter : JsonConverter<AggregateId>
{
  public override AggregateId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? default : new AggregateId(value);
  }

  public override void Write(Utf8JsonWriter writer, AggregateId value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.Value);
  }
}
