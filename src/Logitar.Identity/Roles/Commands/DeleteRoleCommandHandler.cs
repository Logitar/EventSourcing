using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Roles.Commands;

/// <summary>
/// The handler for <see cref="DeleteRoleCommand"/> commands.
/// </summary>
internal class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Role>
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
  /// Initializes a new instance of the <see cref="DeleteRoleCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorContext">The actor context.</param>
  /// <param name="eventStore">The event store.</param>
  /// <param name="roleQuerier">The role querier.</param>
  public DeleteRoleCommandHandler(IActorContext actorContext,
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
  /// <returns>The deleted role.</returns>
  /// <exception cref="AggregateNotFoundException{RoleAggregate}">The specified role could not be found.</exception>
  /// <exception cref="InvalidOperationException">The role output could not be found.</exception>
  public async Task<Role> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    RoleAggregate role = await _eventStore.LoadAsync<RoleAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<RoleAggregate>(id);
    Role output = await _roleQuerier.GetAsync(role.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The role output (Id={role.Id}) could not be found.");

    role.Delete(_actorContext.ActorId);

    await _eventStore.SaveAsync(role, cancellationToken);

    return output;
  }
}
