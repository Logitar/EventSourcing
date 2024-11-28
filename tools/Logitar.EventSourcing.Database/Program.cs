using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Database;

internal class Program
{
  public static void Main(string[] args)
  {
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
    builder.Services.AddHostedService<Worker>();

    DatabaseProvider? databaseProvider = builder.Configuration.GetValue<DatabaseProvider?>("DatabaseProvider");
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        builder.Services.AddDbContext<EventContext>(builder => builder.UseNpgsql(b => b.MigrationsAssembly("Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL")));
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        builder.Services.AddDbContext<EventContext>(builder => builder.UseSqlServer(b => b.MigrationsAssembly("Logitar.EventSourcing.EntityFrameworkCore.SqlServer")));
        break;
    }

    IHost host = builder.Build();
    host.Run();
  }
}
