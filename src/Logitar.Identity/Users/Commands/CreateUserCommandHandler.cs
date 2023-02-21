using Logitar.EventSourcing;
using Logitar.Identity.Realms;
using Logitar.Identity.Roles;
using MediatR;
using System.Globalization;

namespace Logitar.Identity.Users.Commands;

/// <summary>
/// The handler for <see cref="CreateUserCommand"/> commands.
/// </summary>
internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
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
  /// The password helper.
  /// </summary>
  private readonly IPasswordHelper _passwordHelper;
  /// <summary>
  /// The user helper.
  /// </summary>
  private readonly IUserHelper _userHelper;
  /// <summary>
  /// The user querier.
  /// </summary>
  private readonly IUserQuerier _userQuerier;
  /// <summary>
  /// The user repository.
  /// </summary>
  private readonly IUserRepository _userRepository;

  /// <summary>
  /// Initializes a new instance of the <see cref="CreateUserCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorContext">The actor context.</param>
  /// <param name="eventStore">The event store.</param>
  /// <param name="passwordHelper">The password helper.</param>
  /// <param name="userHelper">The user helper.</param>
  /// <param name="userQuerier">The user querier.</param>
  /// <param name="userRepository">The user repository.</param>
  public CreateUserCommandHandler(IActorContext actorContext,
    IEventStore eventStore,
    IPasswordHelper passwordHelper,
    IUserHelper userHelper,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _actorContext = actorContext;
    _eventStore = eventStore;
    _passwordHelper = passwordHelper;
    _userHelper = userHelper;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The created user.</returns>
  /// /// <exception cref="AggregateNotFoundException">The specified realm could not be found.</exception>
  /// <exception cref="UniqueNameAlreadyUsedException">The specified unique name is already used.</exception>
  /// <exception cref="InvalidOperationException">The user output could not be found.</exception>
  public async Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken)
  {
    CreateUserInput input = command.Input;

    AggregateId realmId = new(input.RealmId);
    RealmAggregate realm = await _eventStore.LoadAsync<RealmAggregate>(realmId, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(realmId, nameof(input.RealmId));

    if (await _userRepository.LoadAsync(realm, input.Username, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException(input.Username, nameof(input.Username));
    }

    string? passwordHash = input.Password == null ? null : _passwordHelper.ValidateAndHash(realm, input.Password);
    Gender? gender = input.Gender == null ? null : new Gender(input.Gender);
    CultureInfo? locale = input.Locale?.GetCultureInfo();
    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();
    IEnumerable<RoleAggregate>? roles = await _userHelper.GetRolesAsync(realm, input, cancellationToken);

    UserAggregate user = new(_actorContext.ActorId, realm, input.Username, passwordHash,
      input.FirstName, input.MiddleName, input.LastName, input.Nickname, input.Birthdate, gender,
      locale, input.TimeZone, input.Picture, input.Profile, input.Website, customAttributes, roles);

    await _eventStore.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The user output (Id={user.Id}) could not be found.");
  }
}
