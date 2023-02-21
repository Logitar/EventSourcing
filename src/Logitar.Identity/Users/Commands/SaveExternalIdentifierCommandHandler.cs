using Logitar.EventSourcing;
using Logitar.Identity.Realms;
using MediatR;

namespace Logitar.Identity.Users.Commands;

/// <summary>
/// The handler for <see cref="SaveExternalIdentifierCommand"/> commands.
/// </summary>
internal class SaveExternalIdentifierCommandHandler : IRequestHandler<SaveExternalIdentifierCommand, User>
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
  /// The user querier.
  /// </summary>
  private readonly IUserQuerier _userQuerier;
  /// <summary>
  /// The user repository.
  /// </summary>
  private readonly IUserRepository _userRepository;

  /// <summary>
  /// Initializes a new instance of the <see cref="SaveExternalIdentifierCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorContext<">The actor context.</param>
  /// <param name="eventStore">The event store.</param>
  /// <param name="userQuerier">The user querier.</param>
  /// <param name="userRepository">The user repository.</param>
  public SaveExternalIdentifierCommandHandler(IActorContext actorContext,
    IEventStore eventStore,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _actorContext = actorContext;
    _eventStore = eventStore;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated user.</returns>
  /// <exception cref="AggregateNotFoundException{UserAggregate}">The specified user could not be found.</exception>
  /// <exception cref="InvalidOperationException">The user's realm or user output could not be found.</exception>
  /// <exception cref="ExternalIdentifierAlreadyUsed">The external identifier is already used by another user.</exception>
  public async Task<User> Handle(SaveExternalIdentifierCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    UserAggregate user = await _eventStore.LoadAsync<UserAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(id);
    RealmAggregate realm = await _eventStore.LoadAsync<RealmAggregate>(user.RealmId, cancellationToken)
      ?? throw new InvalidOperationException($"The realm 'Id={user.RealmId}' could not be found.");

    if (command.Value != null)
    {
      UserAggregate? otherUser = await _userRepository.LoadAsync(realm, command.Key, command.Value, cancellationToken);
      if (!user.Equals(otherUser))
      {
        throw new ExternalIdentifierAlreadyUsed(user, command.Key, command.Value);
      }
    }

    user.SaveExternalIdentifier(_actorContext.ActorId, command.Key, command.Value);

    await _eventStore.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The user output (Id={user.Id}) could not be found.");
  }
}
