
using Logitar.EventSourcing.Demo.ReadModel;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo;

public class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    if (application.Configuration.GetValue<bool>("MigrateDatabase"))
    {
      using IServiceScope scope = application.Services.CreateScope();

      using EventContext eventContext = scope.ServiceProvider.GetRequiredService<EventContext>();
      await eventContext.Database.MigrateAsync();

      using TodoContext todoContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
      await todoContext.Database.MigrateAsync();
    }

    application.Run();
  }
}