using FluentValidation;

namespace Logitar.EventSourcing.Demo.Domain;

public record Url
{
  public const int MaximumLength = 2048;

  public string Value { get; }

  public Url(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Url? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Url>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Url();
    }
  }
}
