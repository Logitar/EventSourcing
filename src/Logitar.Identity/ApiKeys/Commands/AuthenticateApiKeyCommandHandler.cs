using Logitar.EventSourcing;
using Logitar.Identity.Accounts;
using MediatR;

namespace Logitar.Identity.ApiKeys.Commands;

/// <summary>
/// The handler for <see cref="AuthenticateApiKeyCommand"/> commands.
/// </summary>
internal class AuthenticateApiKeyCommandHandler : IRequestHandler<AuthenticateApiKeyCommand, ApiKey>
{
  /// <summary>
  /// The API key helper.
  /// </summary>
  private readonly IApiKeyHelper _apiKeyHelper;
  /// <summary>
  /// The API key querier.
  /// </summary>
  private readonly IApiKeyQuerier _apiKeyQuerier;
  /// <summary>
  /// The cache service.
  /// </summary>
  private readonly ICacheService _cacheService;
  /// <summary>
  /// The event store.
  /// </summary>
  private readonly IEventStore _eventStore;

  /// <summary>
  /// Initializes a new instance of the <see cref="AuthenticateApiKeyCommandHandler"/> class.
  /// </summary>
  /// <param name="apiKeyHelper">The API key helper.</param>
  /// <param name="apiKeyQuerier">The API key querier.</param>
  /// <param name="cacheService">The cache service.</param>
  /// <param name="eventStore">The event store.</param>
  public AuthenticateApiKeyCommandHandler(IApiKeyHelper apiKeyHelper,
    IApiKeyQuerier apiKeyQuerier,
    ICacheService cacheService,
    IEventStore eventStore)
  {
    _apiKeyHelper = apiKeyHelper;
    _apiKeyQuerier = apiKeyQuerier;
    _cacheService = cacheService;
    _eventStore = eventStore;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="request">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The created API key.</returns>
  /// <exception cref="InvalidCredentialsException">The specified API key is not valid or could not be authenticated.</exception>
  /// <exception cref="ApiKeyIsExpiredException">The API key is expired.</exception>
  public async Task<ApiKey> Handle(AuthenticateApiKeyCommand request, CancellationToken cancellationToken)
  {
    XApiKey xApiKey;
    try
    {
      xApiKey = XApiKey.Parse(request.XApiKey, request.Prefix);
    }
    catch (Exception innerException)
    {
      throw new InvalidCredentialsException(innerException);
    }

    AggregateId id = new(xApiKey.Id);
    CachedApiKey? cached = _cacheService.GetApiKey(id);

    ApiKeyAggregate? apiKey = cached?.Aggregate ?? await _eventStore.LoadAsync<ApiKeyAggregate>(id, cancellationToken);
    if (apiKey == null || !_apiKeyHelper.IsMatch(apiKey, xApiKey.Secret))
    {
      throw new InvalidCredentialsException();
    }
    else if (apiKey.IsExpired())
    {
      throw new ApiKeyIsExpiredException(apiKey);
    }

    ApiKey output = await _apiKeyQuerier.GetAsync(apiKey.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The API key output (Id={apiKey.Id}) could not be found.");

    _cacheService.SetApiKey(apiKey, output);

    return output;
  }
}
