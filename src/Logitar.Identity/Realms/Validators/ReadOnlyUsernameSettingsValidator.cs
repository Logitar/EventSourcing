using FluentValidation;

namespace Logitar.Identity.Realms.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="ReadOnlyUsernameSettings"/> class.
/// </summary>
internal class ReadOnlyUsernameSettingsValidator : AbstractValidator<ReadOnlyUsernameSettings>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ReadOnlyUsernameSettingsValidator"/> class.
  /// </summary>
  public ReadOnlyUsernameSettingsValidator()
  {
    RuleFor(x => x.AllowedCharacters).NullOrNotEmpty();
  }
}
