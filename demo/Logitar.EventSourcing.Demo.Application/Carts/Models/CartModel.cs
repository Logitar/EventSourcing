namespace Logitar.EventSourcing.Demo.Application.Carts.Models;

public class CartModel : AggregateModel
{
  public new Guid Id
  {
    get => Guid.Parse(base.Id);
    set => base.Id = value.ToString();
  }

  public List<ItemModel> Items { get; set; } = [];
}
