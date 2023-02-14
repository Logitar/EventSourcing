using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

internal class CarConverter : JsonConverter<Car?>
{
  private const char Separator = '|';

  public override Car? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string[]? values = reader.GetString()?.Split(Separator);
    if (values?.Length != 4)
    {
      return null;
    }

    Person? owner = string.IsNullOrEmpty(values[3]) ? null : new Person(values[3]);

    return new Car(int.Parse(values[0]), values[1], values[2], owner);
  }

  public override void Write(Utf8JsonWriter writer, Car? car, JsonSerializerOptions options)
  {
    writer.WriteStringValue(car == null ? null : string.Join(Separator, car.Year, car.Make, car.Model, car.Owner?.FullName));
  }
}
