using FluentValidation;
using Logitar.Identity.Accounts;

namespace Logitar.Identity.Realms.Validators;

/// <summary>
/// The validator used to validate instances of <see cref="ExternalProviderConfiguration"/> classes.
/// </summary>
internal class ExternalProviderConfigurationValidator : AbstractValidator<KeyValuePair<ExternalProvider, ExternalProviderConfiguration>>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ExternalProviderConfigurationValidator"/> class.
  /// </summary>
  public ExternalProviderConfigurationValidator()
  {
    When(x => x.Key == ExternalProvider.GoogleOAuth2, () =>
    {
      RuleFor(x => x.Value).Must(x => x is ReadOnlyGoogleOAuth2Configuration)
        .WithMessage("'{PropertyName}' must be of type ReadOnlyGoogleOAuth2Configuration.");

      When(x => x.Value is ReadOnlyGoogleOAuth2Configuration, () =>
      {
        RuleFor(x => (ReadOnlyGoogleOAuth2Configuration)x.Value).SetValidator(new ReadOnlyGoogleOAuth2ConfigurationValidator());
      });
    });
  }
}
