using FluentValidation;
using Logitar.Identity.Realms.Events;

namespace Logitar.Identity.Realms.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="RealmUpdatedEvent"/> class.
/// </summary>
internal class RealmUpdatedValidator : AbstractValidator<RealmUpdatedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RealmUpdatedValidator"/> class.
  /// </summary>
  public RealmUpdatedValidator()
  {
    RuleFor(x => x.DisplayName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Description).NullOrNotEmpty();

    RuleFor(x => x.DefaultLocale).Locale();

    RuleFor(x => x.Url).NullOrNotEmpty()
      .Url();

    RuleFor(x => x.UsernameSettings).SetValidator(new ReadOnlyUsernameSettingsValidator());

    RuleFor(x => x.PasswordSettings).SetValidator(new ReadOnlyPasswordSettingsValidator());

    RuleFor(x => x.JwtSecret).NotEmpty()
      .MinimumLength(256 / 8);

    RuleForEach(x => x.CustomAttributes.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.CustomAttributes.Values).NotEmpty();

    When(x => x.GoogleOAuth2Configuration != null,
      () => RuleFor(x => x.GoogleOAuth2Configuration!).SetValidator(new ReadOnlyGoogleOAuth2ConfigurationValidator()));
  }
}
