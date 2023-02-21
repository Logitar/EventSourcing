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
  /// <param name="defaultRegion">The default region used to validate phone numbers.</param>
  public PhoneUpdatedValidator(string defaultRegion)
  {
    RuleFor(x => x.CountryCode).NullOrNotEmpty()
      .MaximumLength(10);

    RuleFor(x => x.Number).NotEmpty()
      .MaximumLength(20);

    RuleFor(x => x.Extension).NullOrNotEmpty()
      .MaximumLength(10);

    RuleFor(p => p).PhoneNumber(defaultRegion);
  }
}
