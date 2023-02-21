using Logitar.EventSourcing;
using Logitar.Identity.Users;
using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The handler for <see cref="CreatePhoneCommand"/> commands.
/// </summary>
internal class CreatePhoneCommandHandler : IRequestHandler<CreatePhoneCommand, Phone>
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
  /// Initializes a new instance of the <see cref="CreatePhoneCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorContext">The actor context.</param>
  /// <param name="eventStore">The event store.</param>
  /// <param name="phoneQuerier">The phone number querier.</param>
  /// <param name="phoneRepository">The phone number repository.</param>
  public CreatePhoneCommandHandler(IActorContext actorContext,
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
  /// <returns>The created phone number.</returns>
  /// <exception cref="AggregateNotFoundException">The specified user could not be found.</exception>
  /// <exception cref="InvalidOperationException">The phone number output could not be found.</exception>
  public async Task<Phone> Handle(CreatePhoneCommand command, CancellationToken cancellationToken)
  {
    CreatePhoneInput input = command.Input;

    AggregateId userId = new(input.UserId);
    UserAggregate user = await _eventStore.LoadAsync<UserAggregate>(userId, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(userId, nameof(input.UserId));

    List<PhoneAggregate> changedPhones = new(capacity: 2);
    bool isDefault = input.IsDefault;
    if (isDefault)
    {
      PhoneAggregate? @default = await _phoneRepository.LoadDefaultAsync(user.Id, cancellationToken);
      if (@default != null)
      {
        @default.SetDefault(_actorContext.ActorId, isDefault: false);
        changedPhones.Add(@default);
      }
    }
    else
    {
      isDefault = !(await _phoneRepository.LoadByUserAsync(user.Id, cancellationToken)).Any();
    }

    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();

    PhoneAggregate phone = new(_actorContext.ActorId, user, input.Number, input.CountryCode,
      input.Extension, input.IsArchived, isDefault, input.IsVerified, input.Label, customAttributes);
    changedPhones.Add(phone);

    await _eventStore.SaveAsync(changedPhones, cancellationToken);

    return await _phoneQuerier.GetAsync(phone.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The phone number output (Id={phone.Id}) could not be found.");
  }
}
