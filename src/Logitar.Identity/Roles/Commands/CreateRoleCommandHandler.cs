using Logitar.EventSourcing;
using Logitar.Identity.Realms;
using MediatR;

namespace Logitar.Identity.Roles.Commands;

/// <summary>
/// The handler for <see cref="CreateRoleCommand"/> commands.
/// </summary>
internal class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Role>
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
  /// The role querier.
  /// </summary>
  private readonly IRoleQuerier _roleQuerier;
  /// <summary>
  /// The role repository.
  /// </summary>
  private readonly IRoleRepository _roleRepository;

  /// <summary>
  /// Initializes a new instance of the <see cref="CreateRoleCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorContext">The actor context.</param>
  /// <param name="eventStore">The event store.</param>
  /// <param name="roleQuerier">The role querier.</param>
  /// <param name="roleRepository">The role repository.</param>
  public CreateRoleCommandHandler(IActorContext actorContext,
    IEventStore eventStore,
    IRoleQuerier roleQuerier,
    IRoleRepository roleRepository)
  {
    _actorContext = actorContext;
    _eventStore = eventStore;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The created role.</returns>
  /// <exception cref="AggregateNotFoundException">The specified realm could not be found.</exception>
  /// <exception cref="UniqueNameAlreadyUsedException">The specified unique name is already used.</exception>
  /// <exception cref="InvalidOperationException">The role output could not be found.</exception>
  public async Task<Role> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
  {
    CreateRoleInput input = command.Input;

    AggregateId realmId = new(input.RealmId);
    RealmAggregate realm = await _eventStore.LoadAsync<RealmAggregate>(realmId, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(realmId, nameof(input.RealmId));

    if (await _roleRepository.LoadAsync(realm, input.UniqueName, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException(input.UniqueName, nameof(input.UniqueName));
    }

    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();

    RoleAggregate role = new(_actorContext.ActorId, realm, input.UniqueName, input.DisplayName,
      input.Description, customAttributes);

    await _eventStore.SaveAsync(role, cancellationToken);

    return await _roleQuerier.GetAsync(role.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The role output (Id={role.Id}) could not be found.");
  }
}
