using Logitar.EventSourcing.Demo.Domain.Errors;
using Logitar.EventSourcing.Demo.Domain.Products;

namespace Logitar.EventSourcing.Demo.Application.Products;

public class ProductNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified product could not be found.";

  public Guid ProductId
  {
    get => (Guid)Data[nameof(ProductId)]!;
    private set => Data[nameof(ProductId)] = value;
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
      error.AddData(nameof(ProductId), ProductId);
      error.AddData(nameof(PropertyName), PropertyName);
      return error;
    }
  }

  public ProductNotFoundException(ProductId productId, string propertyName) : base(BuildMessage(productId, propertyName))
  {
    ProductId = productId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(ProductId productId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ProductId), productId.ToGuid())
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
