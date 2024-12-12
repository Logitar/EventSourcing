using Logitar.EventSourcing.Demo.Application;
using Logitar.Security.Claims;

namespace Logitar.EventSourcing.Demo;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  protected HttpContext HttpContext => _httpContextAccessor.HttpContext
    ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} has not been initialized.");

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public ActorId? ActorId
  {
    get
    {
      Claim[] claims = HttpContext.User.FindAll(Rfc7519ClaimNames.Subject).ToArray();
      if (claims.Length == 0)
      {
        return null;
      }
      else if (claims.Length > 1)
      {
        throw new InvalidOperationException($"Only a single '{Rfc7519ClaimNames.Subject}' claim is expected.");
      }

      return new ActorId(claims.Single().Value);
    }
  }
}
