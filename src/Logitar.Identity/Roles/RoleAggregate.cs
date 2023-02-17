using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Realms;
using Logitar.Identity.Roles.Events;
using Logitar.Identity.Roles.Validators;

namespace Logitar.Identity.Roles;

/// <summary>
/// The domain aggregate representing a role. Roles can be used to control user permissions. You can
/// authorize actions by limiting the roles that can perform each action. Roles can also be used as
/// user groups. Roles must belong to a realm.
/// </summary>
public class RoleAggregate : AggregateRoot
{
  /// <summary>
  /// The custom attributes of the role.
  /// </summary>
  private readonly Dictionary<string, string> _customAttributes = new();

  /// <summary>
  /// Initializes a new instance of the <see cref="RoleAggregate"/> class using the specified aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  public RoleAggregate(AggregateId id) : base(id)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="RoleAggregate"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorId">The identifier of the actor creating the role.</param>
  /// <param name="realm">The realm in which the role belongs.</param>
  /// <param name="uniqueName">The unique name of the role.</param>
  /// <param name="displayName">The display name of the role.</param>
  /// <param name="description">A textual description for the role.</param>
  /// <param name="customAttributes">The custom attributes of the role.</param>
  public RoleAggregate(AggregateId actorId, RealmAggregate realm, string uniqueName, string? displayName = null,
    string? description = null, Dictionary<string, string>? customAttributes = null) : base()
  {
    RoleCreatedEvent e = new()
    {
      ActorId = actorId,
      RealmId = realm.Id,
      UniqueName = uniqueName.CleanTrim() ?? string.Empty,
      DisplayName = displayName?.CleanTrim(),
      Description = description?.CleanTrim(),
      CustomAttributes = customAttributes ?? new()
    };
    new RoleCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

  /// <summary>
  /// Gets the identifier of the realm in which the role belongs.
  /// </summary>
  public AggregateId RealmId { get; private set; }

  /// <summary>
  /// Gets the unique name of the role (not case-sensitive).
  /// </summary>
  public string UniqueName { get; private set; } = string.Empty;
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
  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  /// <summary>
  /// Applies the specified event to the role.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(RoleCreatedEvent e)
  {
    RealmId = e.RealmId;

    UniqueName = e.UniqueName;
    DisplayName = e.DisplayName;
    Description = e.Description;

    _customAttributes.Clear();
    _customAttributes.AddRange(e.CustomAttributes);
  }

  /// <summary>
  /// Deletes the role.
  /// </summary>
  /// <param name="actorId">The identifier of the actor deleting the role.</param>
  public void Delete(AggregateId actorId)
  {
    ApplyChange(new RoleDeletedEvent
    {
      ActorId = actorId
    });
  }
  /// <summary>
  /// Applies the specified event to the role.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(RoleDeletedEvent e)
  {
  }

  /// <summary>
  /// Updates the role using the specified arguments.
  /// </summary>
  /// <param name="actorId">The identifier of the actor creating the role.</param>
  /// <param name="displayName">The display name of the role.</param>
  /// <param name="description">A textual description for the role.</param>
  /// <param name="customAttributes">The custom attributes of the role.</param>
  public void Update(AggregateId actorId, string? displayName, string? description,
    Dictionary<string, string>? customAttributes)
  {
    RoleUpdatedEvent e = new()
    {
      ActorId = actorId,
      DisplayName = displayName?.CleanTrim(),
      Description = description?.CleanTrim(),
      CustomAttributes = customAttributes ?? new()
    };
    new RoleUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  /// <summary>
  /// Applies the specified event to the role.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(RoleUpdatedEvent e)
  {
    DisplayName = e.DisplayName;
    Description = e.Description;

    _customAttributes.Clear();
    _customAttributes.AddRange(e.CustomAttributes);
  }

  /// <summary>
  /// Returns a string representation of the current role.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
