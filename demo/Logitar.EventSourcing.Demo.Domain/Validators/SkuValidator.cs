using FluentValidation;
using FluentValidation.Validators;

namespace Logitar.EventSourcing.Demo.Domain.Validators;

internal class SkuValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "SkuValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' must be composed of alphanumeric non-empty words separated by hyphens (-).";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    return value.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));
  }
}
