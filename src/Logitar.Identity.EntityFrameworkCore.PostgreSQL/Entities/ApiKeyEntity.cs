using Logitar.Identity.ApiKeys.Events;
using System.Text.Json;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model representing an API key.
/// </summary>
internal class ApiKeyEntity : AggregateEntity, ICustomAttributes
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyEntity"/> class using the specified arguments.
  /// </summary>
  /// <param name="e">The creation event.</param>
  /// <param name="realm">The realm the API key belongs to.</param>
  public ApiKeyEntity(ApiKeyCreatedEvent e, RealmEntity realm) : base(e)
  {
    Realm = realm;
    RealmId = realm.RealmId;

    SecretHash = e.SecretHash;

    Title = e.Title;
    Description = e.Description;

    ExpiresOn = e.ExpiresOn;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyEntity"/> class.
  /// </summary>
  private ApiKeyEntity()
  {
  }

  /// <summary>
  /// Gets the primary identifier of the API key.
  /// </summary>
  public int ApiKeyId { get; private set; }

  /// <summary>
  /// Gets the realm in which the API key belongs.
  /// </summary>
  public RealmEntity? Realm { get; set; }
  /// <summary>
  /// Gets the identifier of the realm in which the API key belongs.
  /// </summary>
  public int RealmId { get; private set; }

  /// <summary>
  /// Gets the salted and hashed secret of the API key.
  /// </summary>
  public string? SecretHash { get; private set; }

  /// <summary>
  /// Gets the title (or display name) of the API key.
  /// </summary>
  public string Title { get; set; } = string.Empty;
  /// <summary>
  /// Gets a textual description for the API key.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Gets the date and time when the API key will expire.
  /// </summary>
  public DateTime? ExpiresOn { get; set; }

  /// <summary>
  /// Gets the custom attributes of the API key.
  /// </summary>
  public string? CustomAttributes { get; private set; }

  /// <summary>
  /// Gets the list of roles (scopes) of the API key.
  /// </summary>
  public List<RoleEntity> Roles { get; private set; } = new();

  /// <summary>
  /// Updates the API key to the state of the specified event.
  /// </summary>
  /// <param name="e">The update event.</param>
  public void Update(ApiKeyUpdatedEvent e)
  {
    base.Update(e);

    Title = e.Title;
    Description = e.Description;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }
}
