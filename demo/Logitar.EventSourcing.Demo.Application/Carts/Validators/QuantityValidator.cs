using FluentValidation;
using Logitar.EventSourcing.Demo.Application.Carts.Models;

namespace Logitar.EventSourcing.Demo.Application.Carts.Validators;

internal class QuantityValidator : AbstractValidator<QuantityPayload>
{
  public QuantityValidator()
  {
    RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
  }
}
