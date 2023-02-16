using Logitar.EventSourcing;
using Logitar.Identity.Realms;
using Logitar.Identity.Roles;
using MediatR;
using System.Globalization;

namespace Logitar.Identity.Users.Commands;

/// <summary>
/// The handler for <see cref="UpdateUserCommand"/> commands.
/// </summary>
internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
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
  /// The user helper.
  /// </summary>
  private readonly IUserHelper _userHelper;
  /// <summary>
  /// The user querier.
  /// </summary>
  private readonly IUserQuerier _userQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="UpdateUserCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="eventStore">The event store.</param>
  /// <param name="identityContext">The identity context.</param>
  /// <param name="passwordService">The password service.</param>
  /// <param name="userHelper">The user helper.</param>
  /// <param name="userQuerier">The user querier.</param>
  public UpdateUserCommandHandler(IEventStore eventStore,
    IIdentityContext identityContext,
    IPasswordService passwordService,
    IUserHelper userHelper,
    IUserQuerier userQuerier)
  {
    _eventStore = eventStore;
    _identityContext = identityContext;
    _passwordService = passwordService;
    _userHelper = userHelper;
    _userQuerier = userQuerier;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated user.</returns>
  /// <exception cref="AggregateNotFoundException{UserAggregate}">The specified user could not be found.</exception>
  /// <exception cref="InvalidOperationException">The user's realm or user output could not be found.</exception>
  public async Task<User> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    UserAggregate user = await _eventStore.LoadAsync<UserAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(id);
    RealmAggregate realm = await _eventStore.LoadAsync<RealmAggregate>(user.RealmId, cancellationToken)
      ?? throw new InvalidOperationException($"The realm 'Id={user.RealmId}' could not be found.");

    UpdateUserInput input = command.Input;

    Gender? gender = input.Gender == null ? null : new Gender(input.Gender);
    CultureInfo? locale = input.Locale?.GetCultureInfo();
    string? passwordHash = input.Password == null ? null : _passwordService.ValidateAndHash(realm, input.Password);
    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();
    IEnumerable<RoleAggregate>? roles = await _userHelper.GetRolesAsync(realm, input, cancellationToken);

    user.Update(_identityContext.ActorId, passwordHash, input.FirstName, input.MiddleName, input.LastName,
      input.Nickname, input.Birthdate, gender, locale, input.TimeZone, input.Picture, input.Profile,
      input.Website, customAttributes, roles);

    await _eventStore.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The user output (Id={user.Id}) could not be found.");
  }
}
