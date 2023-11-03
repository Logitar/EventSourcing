using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.EventSourcing.EntityFrameworkCore.SqlServer;

[Trait(Traits.Category, Categories.Integration)]
public class AggregateRepositoryTests : Relational.AggregateRepositoryTests
{
  protected override EventContext CreateEventContext()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    string connectionString = configuration.GetValue<string>("SQLCONNSTR_IntegrationTests")
      ?? throw new InvalidOperationException("The configuration 'SQLCONNSTR_IntegrationTests' has not been set.");

    return new EventContext(new DbContextOptionsBuilder<EventContext>()
      .UseSqlServer(connectionString, options => options.MigrationsAssembly("Logitar.EventSourcing.EntityFrameworkCore.SqlServer"))
      .Options);
  }
}
