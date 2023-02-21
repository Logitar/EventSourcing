using FluentValidation;
using Logitar.Identity.Contacts.Events;

namespace Logitar.Identity.Contacts.Validators;

/// <summary>
/// The validator used to validate instances of the <see cref="ContactCreatedEvent"/> class.
/// </summary>
internal abstract class ContactCreatedValidator<T> : AbstractValidator<T> where T : ContactCreatedEvent
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ContactCreatedValidator{T}"/> class.
  /// </summary>
  public ContactCreatedValidator()
  {
    RuleFor(x => x.Label).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleForEach(x => x.CustomAttributes.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.CustomAttributes.Values).NotEmpty();
  }
}
