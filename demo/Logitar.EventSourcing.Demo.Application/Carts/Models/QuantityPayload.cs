namespace Logitar.EventSourcing.Demo.Application.Carts.Models;

public record QuantityPayload
{
  public int Quantity { get; set; }

  public QuantityPayload()
  {
  }

  public QuantityPayload(int quantity)
  {
    Quantity = quantity;
  }
}
