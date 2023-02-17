using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Users.Commands;

/// <summary>
/// The handler for <see cref="DeleteUserCommand"/> commands.
/// </summary>
internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, User>
{
  /// <summary>
  /// The event store.
  /// </summary>
  private readonly IEventStore _eventStore;
  /// <summary>
  /// The identity context.
  /// </summary>
  private readonly IIdentityContext _identityContext;
  /// <summary>
  /// The user querier.
  /// </summary>
  private readonly IUserQuerier _userQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="DeleteUserCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="eventStore">The event store.</param>
  /// <param name="identityContext">The identity context.</param>
  /// <param name="userQuerier">The user querier.</param>
  public DeleteUserCommandHandler(IEventStore eventStore,
    IIdentityContext identityContext,
    IUserQuerier userQuerier)
  {
    _eventStore = eventStore;
    _identityContext = identityContext;
    _userQuerier = userQuerier;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted user.</returns>
  /// <exception cref="AggregateNotFoundException{UserAggregate}">The specified user could not be found.</exception>
  /// <exception cref="InvalidOperationException">The user output could not be found.</exception>
  public async Task<User> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    UserAggregate user = await _eventStore.LoadAsync<UserAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(id);
    User output = await _userQuerier.GetAsync(user.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The user output (Id={user.Id}) could not be found.");

    user.Delete(_identityContext.ActorId);

    await _eventStore.SaveAsync(user, cancellationToken);

    return output;
  }
}
