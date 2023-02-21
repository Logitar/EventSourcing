using Logitar.Identity.Accounts;

namespace Logitar.Identity.ApiKeys;

/// <summary>
/// The exception thrown when an API key is expired.
/// </summary>
public class ApiKeyIsExpiredException : InvalidCredentialsException
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyIsExpiredException"/> class using the specified arguments.
  /// </summary>
  /// <param name="apiKey">The expired API key.</param>
  public ApiKeyIsExpiredException(ApiKeyAggregate apiKey) : base($"The API key '{apiKey}' is expired.")
  {
    Data["ApiKey"] = apiKey.ToString();
  }
}
