using FluentValidation;
using Logitar.Identity.Contacts.Events;

namespace Logitar.Identity.Contacts.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="PhoneCreatedEvent"/> class.
/// </summary>
internal class PhoneCreatedValidator : ContactCreatedValidator<PhoneCreatedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneCreatedValidator"/> class.
  /// </summary>
  /// <param name="defaultRegion">The default region used to validate phone numbers.</param>
  public PhoneCreatedValidator(string defaultRegion)
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
