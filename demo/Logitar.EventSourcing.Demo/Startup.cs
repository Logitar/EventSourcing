using Logitar.EventSourcing.Demo.Application;
using Logitar.EventSourcing.Demo.Authentication;
using Logitar.EventSourcing.Demo.Constants;
using Logitar.EventSourcing.Demo.Filters;
using Logitar.EventSourcing.Demo.Infrastructure;
using Logitar.EventSourcing.Demo.Infrastructure.PostgreSQL;
using Logitar.EventSourcing.Demo.Infrastructure.SqlServer;
using Logitar.EventSourcing.Demo.Settings;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.SqlServer;
using Logitar.EventSourcing.Kurrent;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Scalar.AspNetCore;

namespace Logitar.EventSourcing.Demo;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly BasicAuthenticationSettings _basicAuthenticationSettings;
  private readonly bool _enableOpenApi;
  private readonly bool _useKurrentEventStore;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _basicAuthenticationSettings = configuration.GetSection(BasicAuthenticationSettings.SectionKey).Get<BasicAuthenticationSettings>() ?? new();
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
    _useKurrentEventStore = configuration.GetValue<bool>("UseKurrentEventStore");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddControllers(options => options.Filters.Add<ExceptionHandling>());
    services.AddProblemDetails();

    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });

    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    services.AddSingleton(_basicAuthenticationSettings);

    services.AddHttpContextAccessor();
    services.AddSingleton<IApplicationContext, HttpApplicationContext>();

    string connectionString;
    if (_useKurrentEventStore)
    {
      connectionString = _configuration.GetValue<string>("ESDBCONNSTR_Demo")
        ?? throw new InvalidOperationException("The configuration key 'ESDBCONNSTR_Demo' is required.");
      services.AddLogitarEventSourcingWithKurrent(connectionString);
    }

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
      ?? throw new InvalidOperationException("The configuration key 'DatabaseProvider' is required.");
    switch (databaseProvider)
    {
      case DatabaseProvider.PostgreSQL:
        connectionString = _configuration.GetValue<string>("POSTGRESQLCONNSTR_Demo")
          ?? throw new InvalidOperationException("The configuration key 'POSTGRESQLCONNSTR_Demo' is required.");
        if (!_useKurrentEventStore)
        {
          services.AddLogitarEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString);
        }
        services.AddLogitarEventSourcingDemoInfrastructureWithPostgreSQL(connectionString);
        break;
      case DatabaseProvider.SqlServer:
        connectionString = _configuration.GetValue<string>("SQLCONNSTR_Demo")
          ?? throw new InvalidOperationException("The configuration key 'SQLCONNSTR_Demo' is required.");
        if (!_useKurrentEventStore)
        {
          services.AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(connectionString);
        }
        services.AddLogitarEventSourcingDemoInfrastructureWithSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
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
      application.MapScalarApiReference();
    }

    application.UseHttpsRedirection();
    application.UseAuthentication();
    application.UseAuthorization();

    application.MapControllers();
  }
}
