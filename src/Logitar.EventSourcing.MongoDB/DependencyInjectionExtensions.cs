using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Logitar.EventSourcing.MongoDB;

/// <summary>
/// Provides extension methods for Dependency Injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers the required dependencies for Event Sourcing using the MongoDB store in the specified dependency container.
  /// </summary>
  /// <param name="services">The dependency container.</param>
  /// <param name="configuration">The configuration of the application.</param>
  /// <returns>The dependency container.</returns>
  public static IServiceCollection AddLogitarEventSourcingMongoDB(this IServiceCollection services, IConfiguration configuration)
  {
    MongoDBSettings mongoDbSettings = configuration.GetSection(MongoDBSettings.SectionKey)
      .Get<MongoDBSettings>() ?? new();

    return services.AddLogitarEventSourcingMongoDB(mongoDbSettings);
  }
  /// <summary>
  /// Registers the required dependencies for Event Sourcing using the MongoDB store in the specified dependency container.
  /// </summary>
  /// <param name="services">The dependency container.</param>
  /// <param name="mongoDbSettings">The connection settings to MongoDB.</param>
  /// <returns>The dependency container.</returns>
  public static IServiceCollection AddLogitarEventSourcingMongoDB(this IServiceCollection services, MongoDBSettings mongoDbSettings)
  {
    MongoClient client = new(mongoDbSettings.ConnectionString);
    IMongoDatabase database = client.GetDatabase(mongoDbSettings.DatabaseName);

    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddSingleton(database)
      .AddScoped<IAggregateRepository, AggregateRepository>();
  }
}
