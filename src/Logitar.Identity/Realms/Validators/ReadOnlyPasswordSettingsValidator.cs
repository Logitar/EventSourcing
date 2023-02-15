using FluentValidation;

namespace Logitar.Identity.Realms.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="ReadOnlyPasswordSettings"/> class.
/// </summary>
internal class ReadOnlyPasswordSettingsValidator : AbstractValidator<ReadOnlyPasswordSettings>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ReadOnlyPasswordSettingsValidator"/> class.
  /// </summary>
  public ReadOnlyPasswordSettingsValidator()
  {
    RuleFor(x => x.RequiredLength).GreaterThanOrEqualTo(1);

    RuleFor(x => x.RequiredUniqueChars).GreaterThanOrEqualTo(1)
      .LessThanOrEqualTo(x => x.RequiredLength);
  }
}
