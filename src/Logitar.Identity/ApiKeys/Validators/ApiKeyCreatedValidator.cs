using FluentValidation;
using Logitar.Identity.ApiKeys.Events;

namespace Logitar.Identity.ApiKeys.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="ApiKeyCreatedEvent"/> class.
/// </summary>
internal class ApiKeyCreatedValidator : AbstractValidator<ApiKeyCreatedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyCreatedValidator"/> class.
  /// </summary>
  public ApiKeyCreatedValidator()
  {
    RuleFor(x => x.Title).NotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Description).NullOrNotEmpty();

    RuleForEach(x => x.CustomAttributes.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.CustomAttributes.Values).NotEmpty();
  }
}
