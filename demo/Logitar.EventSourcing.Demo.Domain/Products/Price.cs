using FluentValidation;

namespace Logitar.EventSourcing.Demo.Domain.Products;

public record Price
{
  public decimal Value { get; }

  public Price(decimal value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public static Price? TryCreate(decimal? value) => value.HasValue ? new(value.Value) : null;

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Price>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Price();
    }
  }
}
