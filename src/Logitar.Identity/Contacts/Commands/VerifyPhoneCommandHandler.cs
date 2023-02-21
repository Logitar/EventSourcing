using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The handler for <see cref="VerifyPhoneCommand"/> commands.
/// </summary>
internal class VerifyPhoneCommandHandler : IRequestHandler<VerifyPhoneCommand, Phone>
{
  /// <summary>
  /// The actor context.
  /// </summary>
  private readonly IActorContext _actorContext;
  /// <summary>
  /// The event store.
  /// </summary>
  private readonly IEventStore _eventStore;
  /// <summary>
  /// The phone number querier.
  /// </summary>
  private readonly IPhoneQuerier _phoneQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="VerifyPhoneCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// /// <param name="actorContext">The actor context.</param>
  /// <param name="eventStore">The event store.</param>
  /// <param name="phoneQuerier">The phone number querier.</param>
  public VerifyPhoneCommandHandler(IActorContext actorContext,
    IEventStore eventStore,
    IPhoneQuerier phoneQuerier)
  {
    _actorContext = actorContext;
    _eventStore = eventStore;
    _phoneQuerier = phoneQuerier;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The verified phone number.</returns>
  /// <exception cref="AggregateNotFoundException{PhoneAggregate}">The specified phone number could not be found.</exception>
  /// <exception cref="InvalidOperationException">The phone number output could not be found.</exception>
  public async Task<Phone> Handle(VerifyPhoneCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    PhoneAggregate phone = await _eventStore.LoadAsync<PhoneAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<PhoneAggregate>(id);

    phone.Verify(_actorContext.ActorId);

    await _eventStore.SaveAsync(phone, cancellationToken);

    return await _phoneQuerier.GetAsync(phone.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The phone number output (Id={phone.Id}) could not be found.");
  }
}
