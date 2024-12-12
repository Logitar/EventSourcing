using Logitar.EventSourcing.Demo.Application.Products;
using Logitar.EventSourcing.Demo.Authentication;
using Logitar.EventSourcing.Demo.Constants;
using Logitar.EventSourcing.Demo.Infrastructure.Repositories;
using Logitar.EventSourcing.Demo.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.EventSourcing.Demo;

internal class Startup : StartupBase
{
  private readonly BasicAuthenticationSettings _basicAuthenticationSettings;
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _basicAuthenticationSettings = configuration.GetSection(BasicAuthenticationSettings.SectionKey).Get<BasicAuthenticationSettings>() ?? new();
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddControllers();

    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });

    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    services.AddSingleton(_basicAuthenticationSettings);

    services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    services.AddScoped<IProductRepository, ProductRepository>();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (builder is WebApplication application)
    {
      Configure(application);
    }
  }
  public void Configure(WebApplication application)
  {
    if (_enableOpenApi)
    {
      application.MapOpenApi();
    }

    application.UseHttpsRedirection();
    application.UseAuthentication();
    application.UseAuthorization();

    application.MapControllers();
  }
}
