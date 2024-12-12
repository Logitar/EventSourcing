using Logitar.EventSourcing.Demo.Domain.Products;

namespace Logitar.EventSourcing.Demo.Application.Products;

public class SkuAlreadyUsedException : Exception
{
  private const string ErrorMessage = "The specified SKU is already used.";

  public IEnumerable<Guid> ProductIds
  {
    get => (IEnumerable<Guid>)Data[nameof(ProductIds)]!;
    private set => Data[nameof(ProductIds)] = value;
  }
  public string Sku
  {
    get => (string)Data[nameof(Sku)]!;
    private set => Data[nameof(Sku)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public SkuAlreadyUsedException(Product product, ProductId conflictId, string propertyName) : base(BuildMessage(product, conflictId, propertyName))
  {
    ProductIds = [product.Id.ToGuid(), conflictId.ToGuid()];
    Sku = product.Sku.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Product product, ProductId conflictId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ProductIds), string.Join(", ", [product.Id.ToGuid(), conflictId.ToGuid()]))
    .AddData(nameof(Sku), product.Sku)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
