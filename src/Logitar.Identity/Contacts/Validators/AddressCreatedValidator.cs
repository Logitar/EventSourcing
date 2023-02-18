using FluentValidation;
using Logitar.Identity.Contacts.Events;
using System.Text.RegularExpressions;

namespace Logitar.Identity.Contacts.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="AddressCreatedEvent"/> class.
/// </summary>
internal class AddressCreatedValidator : ContactCreatedValidator<AddressCreatedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AddressCreatedValidator"/> class.
  /// </summary>
  public AddressCreatedValidator()
  {
    RuleFor(x => x.Line1).NotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Line2).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Locality).NotEmpty()
      .MaximumLength(byte.MaxValue);

    IRuleBuilder<AddressCreatedEvent, string?> postalCodeRules = RuleFor(x => x.PostalCode).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);
    When(x => x.PostalCode != null && PostalAddressHelper.GetCountry(x.Country)?.PostalCode != null, () =>
      postalCodeRules.Matches(x => PostalAddressHelper.GetCountry(x.Country)!.PostalCode, RegexOptions.IgnoreCase)
        .WithErrorCode("PostalCodeValidator")
        .WithMessage(x => $"'{{PropertyName}}' must match the following pattern: {PostalAddressHelper.GetCountry(x.Country)!.PostalCode}"));

    RuleFor(x => x.Country).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Country();

    IRuleBuilder<AddressCreatedEvent, string?> regionRules = RuleFor(x => x.Region).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);
    When(x => x.Region != null && PostalAddressHelper.GetCountry(x.Country)?.Regions != null, () =>
      regionRules.Must((x, region) => region == null || PostalAddressHelper.GetCountry(x.Country)!.Regions!.Contains(region))
        .WithErrorCode("RegionValidator")
        .WithMessage(x => $"'{{PropertyName}}' must be one of the following: {string.Join(", ", PostalAddressHelper.GetCountry(x.Country)!.Regions!)}"));
  }
}
