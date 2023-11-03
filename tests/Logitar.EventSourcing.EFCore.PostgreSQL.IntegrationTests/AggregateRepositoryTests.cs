using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

[Trait(Traits.Category, Categories.Integration)]
public class AggregateRepositoryTests : Relational.AggregateRepositoryTests
{
  protected override EventContext CreateEventContext()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    string connectionString = configuration.GetValue<string>("POSTGRESQLCONNSTR_IntegrationTests")
      ?? throw new InvalidOperationException("The configuration 'POSTGRESQLCONNSTR_IntegrationTests' has not been set.");

    return new EventContext(new DbContextOptionsBuilder<EventContext>()
      .UseNpgsql(connectionString, options => options.MigrationsAssembly("Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL"))
      .Options);
  }
}
