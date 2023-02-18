using FluentValidation;
using Logitar.Identity.Contacts.Events;

namespace Logitar.Identity.Contacts.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="PhoneUpdatedEvent"/> class.
/// </summary>
internal class PhoneUpdatedValidator : ContactUpdatedValidator<PhoneUpdatedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneUpdatedValidator"/> class.
  /// </summary>
  public PhoneUpdatedValidator()
  {
    RuleFor(x => x.CountryCode).NotEmpty()
      .MaximumLength(10);

    RuleFor(x => x.Number).NotEmpty()
      .MaximumLength(20);

    RuleFor(x => x.Extension).NullOrNotEmpty()
      .MaximumLength(10);

    RuleFor(p => p).PhoneNumber();
  }
}
