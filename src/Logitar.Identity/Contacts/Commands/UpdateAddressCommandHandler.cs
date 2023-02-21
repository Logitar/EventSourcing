using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The handler for <see cref="UpdateAddressCommand"/> commands.
/// </summary>
internal class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, Address>
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
  /// Initializes a new instance of the <see cref="UpdateAddressCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// /// <param name="actorContext">The actor context.</param>
  /// <param name="addressQuerier">The postal address querier.</param>
  /// <param name="addressRepository">The postal address repository.</param>
  /// <param name="eventStore">The event store.</param>
  public UpdateAddressCommandHandler(IActorContext actorContext,
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
  /// <returns>The updated postal address.</returns>
  /// <exception cref="AggregateNotFoundException{AddressAggregate}">The specified postal address could not be found.</exception>
  /// <exception cref="InvalidOperationException">The postal address output could not be found.</exception>
  public async Task<Address> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    AddressAggregate address = await _eventStore.LoadAsync<AddressAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<AddressAggregate>(id);

    UpdateAddressInput input = command.Input;

    List<AddressAggregate> changedAddresses = new(capacity: 2);
    bool isDefault = address.IsDefault;
    if (!isDefault && input.SetDefault)
    {
      AddressAggregate? @default = await _addressRepository.LoadDefaultAsync(address.UserId, cancellationToken);
      if (@default != null)
      {
        @default.SetDefault(_actorContext.ActorId, isDefault: false);
        changedAddresses.Add(@default);
      }

      isDefault = true;
    }

    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();

    address.Update(_actorContext.ActorId, input.Line1, input.Locality, input.Country, input.Line2,
      input.PostalCode, input.Region, input.IsArchived, isDefault, input.IsVerified, input.Label,
      customAttributes);
    changedAddresses.Add(address);

    await _eventStore.SaveAsync(changedAddresses, cancellationToken);

    return await _addressQuerier.GetAsync(address.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The postal address output (Id={address.Id}) could not be found.");
  }
}
