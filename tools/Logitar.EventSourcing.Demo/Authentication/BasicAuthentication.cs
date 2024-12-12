using Logitar.EventSourcing.Demo.Constants;
using Logitar.EventSourcing.Demo.Settings;
using Logitar.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Logitar.EventSourcing.Demo.Authentication;

internal class BasicAuthenticationOptions : AuthenticationSchemeOptions;

internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
  private readonly BasicAuthenticationSettings _settings;

  public BasicAuthenticationHandler(BasicAuthenticationSettings settings, IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _settings = settings;
  }

#pragma warning disable CS1998 // NOTE(fpion): telling STFU to the compiler; in a real Web Application, this method would have at least one await since it would verify credentials against an external dependency.
  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (_settings.IsEnabled)
    {
      if (Context.Request.Headers.TryGetValue(Headers.Authorization, out StringValues authorization))
      {
        string? value = authorization.Single();
        if (!string.IsNullOrWhiteSpace(value))
        {
          string[] values = value.Split();
          if (values.Length != 2)
          {
            return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
          }
          else if (values[0] == Schemes.Basic)
          {
            byte[] bytes = Convert.FromBase64String(values[1]);
            string credentials = Encoding.UTF8.GetString(bytes);
            int index = credentials.IndexOf(':');
            if (index <= 0)
            {
              return AuthenticateResult.Fail($"The Basic credentials are not valid: '{credentials}'.");
            }

            string username = credentials[..index];
            string password = credentials[(index + 1)..];
            if (!username.Trim().Equals(_settings.Username?.Trim(), StringComparison.InvariantCultureIgnoreCase) || !password.Equals(_settings.Password))
            {
              return AuthenticateResult.Fail("The credentials did not match.");
            }

            ClaimsIdentity identity = new(Scheme.Name);
            identity.AddClaim(new Claim(Rfc7519ClaimNames.Subject, username));
            identity.AddClaim(new Claim(Rfc7519ClaimNames.Username, username));
            identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, DateTime.Now));

            ClaimsPrincipal principal = new(new ClaimsIdentity(Scheme.Name));
            AuthenticationTicket ticket = new(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
          }
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
#pragma warning restore CS1998
