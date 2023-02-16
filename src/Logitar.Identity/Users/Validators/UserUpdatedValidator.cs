using FluentValidation;
using Logitar.Identity.Users.Events;

namespace Logitar.Identity.Users.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="UserUpdatedEvent"/> class.
/// </summary>
internal class UserUpdatedValidator : AbstractValidator<UserUpdatedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UserUpdatedValidator"/> class.
  /// </summary>
  public UserUpdatedValidator()
  {
    RuleFor(x => x.PasswordHash).NullOrNotEmpty();

    RuleFor(x => x.FirstName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);
    RuleFor(x => x.MiddleName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);
    RuleFor(x => x.LastName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);
    RuleFor(x => x.FullName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue * 3 + 2);
    RuleFor(x => x.Nickname).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Locale).Locale();

    RuleFor(x => x.TimeZone).NullOrNotEmpty()
      .TimeZone();

    RuleFor(x => x.Picture).NullOrNotEmpty()
      .MaximumLength(2048)
      .Url();

    RuleFor(x => x.Profile).NullOrNotEmpty()
      .MaximumLength(2048)
      .Url();

    RuleFor(x => x.Website).NullOrNotEmpty()
      .MaximumLength(2048)
      .Url();

    RuleForEach(x => x.CustomAttributes.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.CustomAttributes.Values).NotEmpty();
  }
}
