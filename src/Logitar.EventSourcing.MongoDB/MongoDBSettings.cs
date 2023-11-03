namespace Logitar.EventSourcing.MongoDB;

/// <summary>
/// Represents the connection settings to MongoDB.
/// </summary>
public record MongoDBSettings
{
  /// <summary>
  /// The key of the configuration section of the settings.
  /// </summary>
  public const string SectionKey = "MongoDB";

  /// <summary>
  /// Gets or sets the connection string, or MongoDB URI, to the MongoDB server.
  /// </summary>
  public string ConnectionString { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the name of the MongoDB database.
  /// </summary>
  public string DatabaseName { get; set; } = string.Empty;
}
