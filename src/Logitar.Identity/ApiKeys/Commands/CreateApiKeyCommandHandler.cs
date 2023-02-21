using Logitar.EventSourcing;
using Logitar.Identity.Realms;
using Logitar.Identity.Roles;
using MediatR;

namespace Logitar.Identity.ApiKeys.Commands;

/// <summary>
/// The handler for <see cref="CreateApiKeyCommand"/> commands.
/// </summary>
internal class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, ApiKey>
{
  /// <summary>
  /// The actor context.
  /// </summary>
  private readonly IActorContext _actorContext;
  /// <summary>
  /// The API key helper.
  /// </summary>
  private readonly IApiKeyHelper _apiKeyHelper;
  /// <summary>
  /// The API key querier.
  /// </summary>
  private readonly IApiKeyQuerier _apiKeyQuerier;
  /// <summary>
  /// The event store.
  /// </summary>
  private readonly IEventStore _eventStore;

  /// <summary>
  /// Initializes a new instance of the <see cref="CreateApiKeyCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorContext">The actor context.</param>
  /// <param name="apiKeyHelper">The API key helper.</param>
  /// <param name="apiKeyQuerier">The API key querier.</param>
  /// <param name="eventStore">The event store.</param>
  public CreateApiKeyCommandHandler(IActorContext actorContext,
    IApiKeyHelper apiKeyHelper,
    IApiKeyQuerier apiKeyQuerier,
    IEventStore eventStore)
  {
    _actorContext = actorContext;
    _apiKeyHelper = apiKeyHelper;
    _apiKeyQuerier = apiKeyQuerier;
    _eventStore = eventStore;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The created API key.</returns>
  /// <exception cref="AggregateNotFoundException">The specified realm could not be found.</exception>
  /// <exception cref="UniqueNameAlreadyUsedException">The specified unique name is already used.</exception>
  /// <exception cref="InvalidOperationException">The API key output could not be found.</exception>
  public async Task<ApiKey> Handle(CreateApiKeyCommand command, CancellationToken cancellationToken)
  {
    CreateApiKeyInput input = command.Input;

    AggregateId realmId = new(input.RealmId);
    RealmAggregate realm = await _eventStore.LoadAsync<RealmAggregate>(realmId, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(realmId, nameof(input.RealmId));

    string secretHash = _apiKeyHelper.GenerateSecret(out byte[] secret);
    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();
    IEnumerable<RoleAggregate>? roles = await _apiKeyHelper.GetRolesAsync(realm, input, cancellationToken);

    ApiKeyAggregate apiKey = new(_actorContext.ActorId, realm, secretHash, input.Title,
      input.Description, input.ExpiresOn, customAttributes, roles);

    await _eventStore.SaveAsync(apiKey, cancellationToken);

    ApiKey output = await _apiKeyQuerier.GetAsync(apiKey.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The API key output (Id={apiKey.Id}) could not be found.");
    output.XApiKey = new XApiKey(input.Prefix, output.Id, secret).ToString();

    return output;
  }
}
