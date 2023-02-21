using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The handler for <see cref="UpdatePhoneCommand"/> commands.
/// </summary>
internal class UpdatePhoneCommandHandler : IRequestHandler<UpdatePhoneCommand, Phone>
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
  /// The phone number repository.
  /// </summary>
  private readonly IPhoneRepository _phoneRepository;

  /// <summary>
  /// Initializes a new instance of the <see cref="UpdatePhoneCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// /// <param name="actorContext">The actor context.</param>
  /// <param name="eventStore">The event store.</param>
  /// <param name="phoneQuerier">The phone number querier.</param>
  /// <param name="phoneRepository">The phone number repository.</param>
  public UpdatePhoneCommandHandler(IActorContext actorContext,
    IEventStore eventStore,
    IPhoneQuerier phoneQuerier,
    IPhoneRepository phoneRepository)
  {
    _actorContext = actorContext;
    _eventStore = eventStore;
    _phoneQuerier = phoneQuerier;
    _phoneRepository = phoneRepository;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated phone number.</returns>
  /// <exception cref="AggregateNotFoundException{PhoneAggregate}">The specified phone number could not be found.</exception>
  /// <exception cref="InvalidOperationException">The phone number output could not be found.</exception>
  public async Task<Phone> Handle(UpdatePhoneCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    PhoneAggregate phone = await _eventStore.LoadAsync<PhoneAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<PhoneAggregate>(id);

    UpdatePhoneInput input = command.Input;

    List<PhoneAggregate> changedPhones = new(capacity: 2);
    bool isDefault = phone.IsDefault;
    if (!isDefault && input.SetDefault)
    {
      PhoneAggregate? @default = await _phoneRepository.LoadDefaultAsync(phone.UserId, cancellationToken);
      if (@default != null)
      {
        @default.SetDefault(_actorContext.ActorId, isDefault: false);
        changedPhones.Add(@default);
      }

      isDefault = true;
    }

    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();

    phone.Update(_actorContext.ActorId, input.Number, input.CountryCode, input.Extension,
      input.IsArchived, isDefault, input.IsVerified, input.Label, customAttributes);
    changedPhones.Add(phone);

    await _eventStore.SaveAsync(changedPhones, cancellationToken);

    return await _phoneQuerier.GetAsync(phone.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The phone number output (Id={phone.Id}) could not be found.");
  }
}
