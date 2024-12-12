using FluentValidation;

namespace Logitar.EventSourcing.Demo.Domain.Products;

public record Sku
{
  public const int MaximumLength = 32;

  public string Value { get; }

  public Sku(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Sku>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Sku();
    }
  }
}
