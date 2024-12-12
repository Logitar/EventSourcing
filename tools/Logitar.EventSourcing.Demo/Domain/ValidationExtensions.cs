using FluentValidation;
using Logitar.EventSourcing.Demo.Domain.Validators;

namespace Logitar.EventSourcing.Demo.Domain;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> Description<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Products.Description.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> DisplayName<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Products.DisplayName.MaximumLength);
  }

  public static IRuleBuilderOptions<T, decimal> Price<T>(this IRuleBuilder<T, decimal> ruleBuilder)
  {
    return ruleBuilder.GreaterThan(0);
  }

  public static IRuleBuilderOptions<T, string> Sku<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Products.Sku.MaximumLength).SetValidator(new SkuValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder, IEnumerable<string>? schemes = null)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Domain.Url.MaximumLength).SetValidator(new UrlValidator<T>(schemes));
  }
}
