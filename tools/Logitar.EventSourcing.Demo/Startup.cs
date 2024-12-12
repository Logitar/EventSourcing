namespace Logitar.EventSourcing.Demo;

internal class Startup : StartupBase
{
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddControllers();

    if (_enableOpenApi)
    {
      services.AddOpenApi();
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
    }

    application.UseHttpsRedirection();

    application.MapControllers();
  }
}
