using Logitar.EventSourcing.Demo.Domain.Carts;

namespace Logitar.EventSourcing.Demo.Infrastructure.Converters;

internal class CartIdConverter : JsonConverter<CartId>
{
  public override CartId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new CartId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, CartId cartId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(cartId.Value);
  }
}
