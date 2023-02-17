using FluentValidation;
using Logitar.Identity.Realms;

namespace Logitar.Identity.Users.Validators;

/// <summary>
/// The validator used to validate passwords.
/// </summary>
internal class PasswordValidator : AbstractValidator<string>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PasswordValidator"/> class using the specified arguments.
  /// </summary>
  /// <param name="passwordSettings">The settings used to validate passwords.</param>
  public PasswordValidator(ReadOnlyPasswordSettings passwordSettings)
  {
    RuleFor(x => x).NotEmpty();

    if (passwordSettings.RequiredLength > 1)
    {
      RuleFor(x => x).MinimumLength(passwordSettings.RequiredLength)
        .WithErrorCode("PasswordTooShort")
        .WithMessage($"Passwords must be at least {passwordSettings.RequiredLength} characters.");
    }

    if (passwordSettings.RequiredUniqueChars > 1)
    {
      RuleFor(x => x).Must(x => x.GroupBy(c => c).Count() >= passwordSettings.RequiredUniqueChars)
        .WithErrorCode("PasswordRequiresUniqueChars")
        .WithMessage($"Passwords must use at least {passwordSettings.RequiredUniqueChars} different characters.");
    }

    if (passwordSettings.RequireNonAlphanumeric)
    {
      RuleFor(x => x).Must(x => x.Any(c => !char.IsLetterOrDigit(c)))
        .WithErrorCode("PasswordRequiresNonAlphanumeric")
        .WithMessage("Passwords must have at least one non alphanumeric character.");
    }

    if (passwordSettings.RequireLowercase)
    {
      RuleFor(x => x).Must(x => x.Any(char.IsLower))
        .WithErrorCode("PasswordRequiresLower")
        .WithMessage("Passwords must have at least one lowercase ('a'-'z').");
    }

    if (passwordSettings.RequireUppercase)
    {
      RuleFor(x => x).Must(x => x.Any(char.IsUpper))
        .WithErrorCode("PasswordRequiresUpper")
        .WithMessage("Passwords must have at least one uppercase ('A'-'Z').");
    }

    if (passwordSettings.RequireDigit)
    {
      RuleFor(x => x).Must(x => x.Any(char.IsDigit))
        .WithErrorCode("PasswordRequiresDigit")
        .WithMessage("Passwords must have at least one digit ('0'-'9').");
    }
  }
}
