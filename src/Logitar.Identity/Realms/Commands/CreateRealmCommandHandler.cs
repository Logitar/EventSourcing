using Logitar.Identity.Accounts;
using MediatR;
using System.Globalization;

namespace Logitar.Identity.Realms.Commands;

/// <summary>
/// The handler for <see cref="CreateRealmCommand"/> commands.
/// </summary>
internal class CreateRealmCommandHandler : IRequestHandler<CreateRealmCommand, Realm>
{
  /// <summary>
  /// The identity context.
  /// </summary>
  private readonly IIdentityContext _identityContext;
  /// <summary>
  /// The realm querier.
  /// </summary>
  private readonly IRealmQuerier _realmQuerier;
  /// <summary>
  /// The realm repository.
  /// </summary>
  private readonly IRealmRepository _realmRepository;

  /// <summary>
  /// Initializes a new instance of the <see cref="CreateRealmCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="identityContext">The identity context.</param>
  /// <param name="realmQuerier">The realm querier.</param>
  /// <param name="realmRepository">The realm repository.</param>
  public CreateRealmCommandHandler(IIdentityContext identityContext,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository)
  {
    _identityContext = identityContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The created realm.</returns>
  /// <exception cref="UniqueNameAlreadyUsedException">The specified unique name is already used.</exception>
  /// <exception cref="InvalidOperationException">The realm output could not be found.</exception>
  public async Task<Realm> Handle(CreateRealmCommand command, CancellationToken cancellationToken)
  {
    CreateRealmInput input = command.Input;

    if (await _realmRepository.LoadAsync(input.UniqueName, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException(input.UniqueName, nameof(input.UniqueName));
    }

    CultureInfo? defaultLocale = input.DefaultLocale?.GetCultureInfo();
    ReadOnlyUsernameSettings? usernameSettings = input.UsernameSettings == null ? null : new(input.UsernameSettings);
    ReadOnlyPasswordSettings? passwordSettings = input.PasswordSettings == null ? null : new(input.PasswordSettings);
    Dictionary<ExternalProvider, ExternalProviderConfiguration> externalProviders = RealmHelper.GetExternalProviders(input.GoogleOAuth2Configuration);
    Dictionary<string, string>? customAttributes = RealmHelper.GetCustomAttributes(input.CustomAttributes);

    RealmAggregate realm = new(_identityContext.ActorId, input.UniqueName, input.DisplayName, input.Description,
      defaultLocale, input.Url, input.RequireConfirmedAccount, input.RequireUniqueEmail,
      usernameSettings, passwordSettings, input.JwtSecret, customAttributes, externalProviders);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.GetAsync(realm.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The realm output (Id={realm.Id}) could not be found.");
  }
}
