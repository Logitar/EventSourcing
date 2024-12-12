using Logitar.EventSourcing.Demo.Infrastructure.Commands;
using MediatR;

namespace Logitar.EventSourcing.Demo;

internal class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    if (application.Configuration.GetValue<bool>("EnableMigrations"))
    {
      using IServiceScope scope = application.Services.CreateScope();
      IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      await mediator.Send(new InitializeDatabaseCommand());
    }

    application.Run();
  }
}
