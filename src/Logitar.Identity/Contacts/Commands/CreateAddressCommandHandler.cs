using Logitar.EventSourcing;
using Logitar.Identity.Users;
using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The handler for <see cref="CreateAddressCommand"/> commands.
/// </summary>
internal class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, Address>
{
  /// <summary>
  /// The actor context.
  /// </summary>
  private readonly IActorContext _actorContext;
  /// <summary>
  /// The postal address querier.
  /// </summary>
  private readonly IAddressQuerier _addressQuerier;
  /// <summary>
  /// The postal address repository.
  /// </summary>
  private readonly IAddressRepository _addressRepository;
  /// <summary>
  /// The event store.
  /// </summary>
  private readonly IEventStore _eventStore;

  /// <summary>
  /// Initializes a new instance of the <see cref="CreateAddressCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorContext">The actor context.</param>
  /// <param name="addressQuerier">The postal address querier.</param>
  /// <param name="addressRepository">The postal address repository.</param>
  /// <param name="eventStore">The event store.</param>
  public CreateAddressCommandHandler(IActorContext actorContext,
    IAddressQuerier addressQuerier,
    IAddressRepository addressRepository,
    IEventStore eventStore)
  {
    _actorContext = actorContext;
    _addressQuerier = addressQuerier;
    _addressRepository = addressRepository;
    _eventStore = eventStore;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The created postal address.</returns>
  /// <exception cref="AggregateNotFoundException">The specified user could not be found.</exception>
  /// <exception cref="InvalidOperationException">The postal address output could not be found.</exception>
  public async Task<Address> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
  {
    CreateAddressInput input = command.Input;

    AggregateId userId = new(input.UserId);
    UserAggregate user = await _eventStore.LoadAsync<UserAggregate>(userId, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(userId, nameof(input.UserId));

    List<AddressAggregate> changedAddresses = new(capacity: 2);
    bool isDefault = input.IsDefault;
    if (isDefault)
    {
      AddressAggregate? @default = await _addressRepository.LoadDefaultAsync(user.Id, cancellationToken);
      if (@default != null)
      {
        @default.SetDefault(_actorContext.ActorId, isDefault: false);
        changedAddresses.Add(@default);
      }
    }
    else
    {
      isDefault = !(await _addressRepository.LoadByUserAsync(user.Id, cancellationToken)).Any();
    }

    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();

    AddressAggregate address = new(_actorContext.ActorId, user, input.Line1, input.Locality,
      input.Country, input.Line2, input.PostalCode, input.Region, input.IsArchived, isDefault,
      input.IsVerified, input.Label, customAttributes);
    changedAddresses.Add(address);

    await _eventStore.SaveAsync(changedAddresses, cancellationToken);

    return await _addressQuerier.GetAsync(address.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The postal postal address output (Id={address.Id}) could not be found.");
  }
}
