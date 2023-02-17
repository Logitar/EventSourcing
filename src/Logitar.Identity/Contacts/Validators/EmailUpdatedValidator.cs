using FluentValidation;
using Logitar.Identity.Contacts.Events;

namespace Logitar.Identity.Contacts.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="EmailUpdatedEvent"/> class.
/// </summary>
internal class EmailUpdatedValidator : ContactUpdatedValidator<EmailUpdatedEvent>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EmailUpdatedValidator"/> class.
  /// </summary>
  public EmailUpdatedValidator()
  {
    RuleFor(x => x.Address).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .EmailAddress();
  }
}
