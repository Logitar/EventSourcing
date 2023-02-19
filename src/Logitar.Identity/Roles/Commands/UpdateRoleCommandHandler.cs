using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Roles.Commands;

/// <summary>
/// The handler for <see cref="UpdateRoleCommand"/> commands.
/// </summary>
internal class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Role>
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
  /// Initializes a new instance of the <see cref="UpdateRoleCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// /// <param name="actorContext">The actor context.</param>
  /// <param name="eventStore">The event store.</param>
  /// <param name="roleQuerier">The role querier.</param>
  public UpdateRoleCommandHandler(IActorContext actorContext,
    IEventStore eventStore,
    IRoleQuerier roleQuerier)
  {
    _actorContext = actorContext;
    _eventStore = eventStore;
    _roleQuerier = roleQuerier;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated role.</returns>
  /// <exception cref="AggregateNotFoundException{RoleAggregate}">The specified role could not be found.</exception>
  /// <exception cref="InvalidOperationException">The role output could not be found.</exception>
  public async Task<Role> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    RoleAggregate role = await _eventStore.LoadAsync<RoleAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<RoleAggregate>(id);

    UpdateRoleInput input = command.Input;

    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();

    role.Update(_actorContext.ActorId, input.DisplayName, input.Description, customAttributes);

    await _eventStore.SaveAsync(role, cancellationToken);

    return await _roleQuerier.GetAsync(role.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The role output (Id={role.Id}) could not be found.");
  }
}
