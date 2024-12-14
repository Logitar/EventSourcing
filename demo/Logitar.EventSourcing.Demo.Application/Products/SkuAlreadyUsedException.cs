using Logitar.EventSourcing.Demo.Domain.Errors;
using Logitar.EventSourcing.Demo.Domain.Products;

namespace Logitar.EventSourcing.Demo.Application.Products;

public class SkuAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified SKU is already used.";

  public Guid ProductId
  {
    get => (Guid)Data[nameof(ProductId)]!;
    private set => Data[nameof(ProductId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
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

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.AddData(nameof(ConflictId), ConflictId);
      error.AddData(nameof(Sku), Sku);
      error.AddData(nameof(PropertyName), PropertyName);
      return error;
    }
  }

  public SkuAlreadyUsedException(Product product, ProductId conflictId, string propertyName) : base(BuildMessage(product, conflictId, propertyName))
  {
    ProductId = product.Id.ToGuid();
    ConflictId = conflictId.ToGuid();
    Sku = product.Sku.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Product product, ProductId conflictId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ProductId), product.Id.ToGuid())
    .AddData(nameof(ConflictId), conflictId.ToGuid())
    .AddData(nameof(Sku), product.Sku)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
