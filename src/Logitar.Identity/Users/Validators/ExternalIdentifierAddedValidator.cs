using FluentValidation;
using Logitar.Identity.Users.Events;

namespace Logitar.Identity.Users.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="ExternalIdentifierAddedEvent"/> class.
/// </summary>
internal class ExternalIdentifierAddedValidator : AbstractValidator<ExternalIdentifierAddedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ExternalIdentifierAddedValidator"/> class.
  /// </summary>
  public ExternalIdentifierAddedValidator()
  {
    RuleFor(x => x.Key).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();

    RuleFor(x => x.Value).NotEmpty()
      .MaximumLength(byte.MaxValue);
  }
}
