using FluentValidation;

namespace Logitar.Identity.Realms.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="ReadOnlyGoogleOAuth2Configuration"/> class.
/// </summary>
internal class ReadOnlyGoogleOAuth2ConfigurationValidator : AbstractValidator<ReadOnlyGoogleOAuth2Configuration>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ReadOnlyGoogleOAuth2ConfigurationValidator"/> class.
  /// </summary>
  public ReadOnlyGoogleOAuth2ConfigurationValidator()
  {
    RuleFor(x => x.ClientId).NotEmpty();
  }
}
