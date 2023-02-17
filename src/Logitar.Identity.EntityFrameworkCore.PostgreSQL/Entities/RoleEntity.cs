using Logitar.Identity.Roles.Events;
using System.Text.Json;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model representing a role.
/// </summary>
internal class RoleEntity : AggregateEntity
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RoleEntity"/> using the specified arguments.
  /// </summary>
  /// <param name="e">The creation event.</param>
  /// <param name="realm">The realm the role belongs to.</param>
  public RoleEntity(RoleCreatedEvent e, RealmEntity realm) : base(e)
  {
    Realm = realm;
    RealmId = realm.RealmId;

    UniqueName = e.UniqueName;
    DisplayName = e.DisplayName;
    Description = e.Description;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="RoleEntity"/> class.
  /// </summary>
  private RoleEntity() : base()
  {
  }

  /// <summary>
  /// Gets the primary identifier of the role.
  /// </summary>
  public int RoleId { get; private set; }

  /// <summary>
  /// Gets the realm in which the role belongs.
  /// </summary>
  public RealmEntity? Realm { get; private set; }
  /// <summary>
  /// Gets the identifier of the realm in which the role belongs.
  /// </summary>
  public int RealmId { get; private set; }

  /// <summary>
  /// Gets the unique name of the role (not case-sensitive).
  /// </summary>
  public string UniqueName { get; private set; } = string.Empty;
  /// <summary>
  /// Gets the normalized unique name of the role (case-sensitive).
  /// </summary>
  public string UniqueNameNormalized
  {
    get => UniqueName.ToUpper();
    private set { }
  }
  /// <summary>
  /// Gets the display name of the role.
  /// </summary>
  public string? DisplayName { get; private set; }
  /// <summary>
  /// Gets a textual description for the role.
  /// </summary>
  public string? Description { get; private set; }

  /// <summary>
  /// Gets the custom attributes of the role.
  /// </summary>
  public string? CustomAttributes { get; private set; }

  /// <summary>
  /// Gets the list of users in this role.
  /// </summary>
  public List<UserEntity> Users { get; private set; } = new();

  /// <summary>
  /// Updates the role to the state of the specified event.
  /// </summary>
  /// <param name="e">The update event.</param>
  public void Update(RoleUpdatedEvent e)
  {
    base.Update(e);

    DisplayName = e.DisplayName;
    Description = e.Description;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }
}
