using FluentValidation;
using Logitar.Identity.Contacts.Events;

namespace Logitar.Identity.Contacts.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="ContactUpdatedEvent"/> class.
/// </summary>
internal abstract class ContactUpdatedValidator<T> : AbstractValidator<T> where T : ContactUpdatedEvent
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ContactUpdatedValidator{T}"/> class.
  /// </summary>
  public ContactUpdatedValidator()
  {
    RuleFor(x => x.Label).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleForEach(x => x.CustomAttributes.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.CustomAttributes.Values).NotEmpty();
  }
}
