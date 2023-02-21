using FluentValidation;
using Logitar.Identity.Contacts.Events;

namespace Logitar.Identity.Contacts.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="EmailCreatedEvent"/> class.
/// </summary>
internal class EmailCreatedValidator : ContactCreatedValidator<EmailCreatedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EmailCreatedValidator"/> class.
  /// </summary>
  public EmailCreatedValidator()
  {
    RuleFor(x => x.Address).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .EmailAddress();
  }
}
