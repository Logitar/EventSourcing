using FluentValidation;
using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Domain;

namespace Logitar.EventSourcing.Demo.Application.Products.Validators;

internal class UpdateProductValidator : AbstractValidator<UpdateProductPayload>
{
  public UpdateProductValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Sku), () => RuleFor(x => x.Sku!).Sku());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Price?.Value != null, () => RuleFor(x => x.Price!.Value!.Value).Price());
    When(x => !string.IsNullOrWhiteSpace(x.PictureUrl?.Value), () => RuleFor(x => x.PictureUrl!.Value!).Url());
  }
}
