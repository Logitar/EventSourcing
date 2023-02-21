using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The handler for <see cref="DeleteAddressCommand"/> commands.
/// </summary>
internal class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Address>
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
  /// Initializes a new instance of the <see cref="DeleteAddressCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorContext">The actor context.</param>
  /// <param name="addressQuerier">The postal address querier.</param>
  /// <param name="addressRepository">The postal address repository.</param>
  /// <param name="eventStore">The event store.</param>
  public DeleteAddressCommandHandler(IActorContext actorContext,
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
  /// <returns>The deleted postal address.</returns>
  /// <exception cref="AggregateNotFoundException{AddressAggregate}">The specified postal address could not be found.</exception>
  /// <exception cref="InvalidOperationException">The postal address output could not be found.</exception>
  public async Task<Address> Handle(DeleteAddressCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    AddressAggregate address = await _eventStore.LoadAsync<AddressAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<AddressAggregate>(id);
    if (address.IsDefault)
    {
      IEnumerable<AddressAggregate> addresses = await _addressRepository.LoadByUserAsync(address.UserId, cancellationToken);
      if (addresses.Any(a => !a.Equals(address)))
      {
        throw new NotImplementedException(); // TODO(fpion): implement
      }
    }

    Address output = await _addressQuerier.GetAsync(address.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The postal address output (Id={address.Id}) could not be found.");

    address.Delete(_actorContext.ActorId);

    await _eventStore.SaveAsync(address, cancellationToken);

    return output;
  }
}
