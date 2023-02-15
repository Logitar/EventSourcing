using FluentValidation;
using Logitar.Identity.Roles.Events;

namespace Logitar.Identity.Roles.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="RoleCreatedEvent"/> class.
/// </summary>
internal class RoleCreatedValidator : AbstractValidator<RoleCreatedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RoleCreatedValidator"/> class.
  /// </summary>
  public RoleCreatedValidator()
  {
    RuleFor(x => x.UniqueName).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();

    RuleFor(x => x.DisplayName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Description).NullOrNotEmpty();

    RuleForEach(x => x.CustomAttributes.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue);
    RuleForEach(x => x.CustomAttributes.Values).NotEmpty();
  }
}
