using Logitar.EventSourcing.Demo.Extensions;
using Logitar.EventSourcing.Demo.ReadModel;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Logitar.EventSourcing.Demo;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddControllers()
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    services.AddOpenApi();

    DatabaseProvider provider = _configuration.GetValue<DatabaseProvider>("DatabaseProvider");
    switch (provider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        string connectionString = _configuration.GetValue<string>("POSTGRESQLCONNSTR_EventContext") ?? string.Empty;
        services.AddEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString);
        break;
      default:
        throw new NotSupportedException($"The database provider '{provider}' is not supported.");
    }

    services.AddDbContext<TodoContext>(options => options.UseNpgsql(_configuration.GetValue<string>("POSTGRESQLCONNSTR_TodoContext")));
    services.AddMediatR(typeof(Startup).Assembly);
    services.AddScoped<IEventBus, EventBus>();
    services.AddScoped<ITodoQuerier, TodoQuerier>();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (builder is WebApplication application)
    {
      if (application.Environment.IsDevelopment())
      {
        application.UseSwagger();
        application.UseSwaggerUI();
      }

      application.UseHttpsRedirection();
      application.MapControllers();
    }
  }
}
