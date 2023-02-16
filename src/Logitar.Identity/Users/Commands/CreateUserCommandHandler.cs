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
  /// The event store.
  /// </summary>
  private readonly IEventStore _eventStore;
  /// <summary>
  /// The identity context.
  /// </summary>
  private readonly IIdentityContext _identityContext;
  /// <summary>
  /// The password service.
  /// </summary>
  private readonly IPasswordService _passwordService;
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
  /// <param name="eventStore">The event store.</param>
  /// <param name="identityContext">The identity context.</param>
  /// <param name="passwordService">The password service.</param>
  /// <param name="userQuerier">The user querier.</param>
  /// <param name="userRepository">The user repository.</param>
  public CreateUserCommandHandler(IEventStore eventStore,
    IIdentityContext identityContext,
    IPasswordService passwordService,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _eventStore = eventStore;
    _identityContext = identityContext;
    _passwordService = passwordService;
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

    Dictionary<AggregateId, RoleAggregate>? roles = null;
    if (input.Roles?.Any() == true)
    {
      IEnumerable<AggregateId> ids = input.Roles.Select(id => new AggregateId(id)).Distinct();
      List<AggregateId> missingRoles = new(capacity: ids.Count());
      List<RoleAggregate> notInRealm = new(capacity: missingRoles.Count);

      roles = (await _eventStore.LoadAsync<RoleAggregate>(ids, cancellationToken))
        .ToDictionary(x => x.Id, x => x);
      foreach (AggregateId id in ids)
      {
        if (!roles.TryGetValue(id, out RoleAggregate? role))
        {
          missingRoles.Add(id);
        }
        else if (role.RealmId != realm.Id)
        {
          notInRealm.Add(role);
        }
      }

      if (missingRoles.Any())
      {
        throw new AggregatesNotFoundException<RoleAggregate>(missingRoles, nameof(input.Roles));
      }
      else if (notInRealm.Any())
      {
        throw new RolesNotInRealmException(notInRealm, realm);
      }
    }

    string? passwordHash = null;
    if (input.Password != null)
    {
      _passwordService.ValidateAndThrow(realm, input.Password);
      passwordHash = _passwordService.Hash(input.Password);
    }

    Gender? gender = input.Gender == null ? null : new Gender(input.Gender);
    CultureInfo? locale = input.Locale?.GetCultureInfo();
    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();

    UserAggregate user = new(_identityContext.ActorId, realm, input.Username, passwordHash,
      input.FirstName, input.MiddleName, input.LastName, input.Nickname, input.Birthdate, gender,
      locale, input.TimeZone, input.Picture, input.Profile, input.Website, customAttributes, roles?.Values);

    await _eventStore.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The user output (Id={user.Id}) could not be found.");
  }
}
