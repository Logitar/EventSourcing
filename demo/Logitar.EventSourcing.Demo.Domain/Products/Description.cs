using FluentValidation;

namespace Logitar.EventSourcing.Demo.Domain.Products;

public record Description
{
  public const int MaximumLength = ushort.MaxValue;

  public string Value { get; }

  public Description(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Description? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Description>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Description();
    }
  }
}
