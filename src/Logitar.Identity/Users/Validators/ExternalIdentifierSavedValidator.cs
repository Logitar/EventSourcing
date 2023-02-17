using FluentValidation;
using Logitar.Identity.Users.Events;

namespace Logitar.Identity.Users.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="ExternalIdentifierSavedEvent"/> class.
/// </summary>
internal class ExternalIdentifierSavedValidator : AbstractValidator<ExternalIdentifierSavedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ExternalIdentifierSavedValidator"/> class.
  /// </summary>
  public ExternalIdentifierSavedValidator()
  {
    RuleFor(x => x.Key).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();

    RuleFor(x => x.Value).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);
  }
}
