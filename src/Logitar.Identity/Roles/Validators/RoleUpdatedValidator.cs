using FluentValidation;
using Logitar.Identity.Roles.Events;

namespace Logitar.Identity.Roles.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="RoleUpdatedEvent"/> class.
/// </summary>
internal class RoleUpdatedValidator : AbstractValidator<RoleUpdatedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RoleUpdatedValidator"/> class.
  /// </summary>
  public RoleUpdatedValidator()
  {
    RuleFor(x => x.DisplayName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Description).NullOrNotEmpty();

    RuleForEach(x => x.CustomAttributes.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue);
    RuleForEach(x => x.CustomAttributes.Values).NotEmpty();
  }
}
