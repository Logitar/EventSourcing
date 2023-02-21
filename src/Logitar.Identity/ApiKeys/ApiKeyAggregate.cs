using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.ApiKeys.Events;
using Logitar.Identity.ApiKeys.Validators;
using Logitar.Identity.Realms;
using Logitar.Identity.Roles;
using System.Data;

namespace Logitar.Identity.ApiKeys;

/// <summary>
/// The domain aggregate representing an API key. API keys are similar to users; they are used to
/// authenticate requests between two systems. API keys can have multiple roles; these are commonly
/// named "scopes". API keys must belong to a realm.
/// </summary>
public class ApiKeyAggregate : AggregateRoot
{
  /// <summary>
  /// The custom attributes of the API key.
  /// </summary>
  private readonly Dictionary<string, string> _customAttributes = new();
  /// <summary>
  /// The role (scope) identifiers of the API key.
  /// </summary>
  private readonly List<AggregateId> _roles = new();

  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyAggregate"/> class using the specified aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  public ApiKeyAggregate(AggregateId id) : base(id)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyAggregate"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorId">The identifier of the actor creating the API key.</param>
  /// <param name="realm">The realm in which the API key belongs.</param>
  /// <param name="secretHash">The salted and hashed secret of the API key.</param>
  /// <param name="title">The title (or display name) of the API key.</param>
  /// <param name="description">A textual description for the API key.</param>
  /// <param name="expiresOn">The date and time when the API key will expire.</param>
  /// <param name="customAttributes">The custom attributes of the API key.</param>
  /// <param name="roles">The roles (or scopes) of the API key.</param>
  public ApiKeyAggregate(AggregateId actorId, RealmAggregate realm, string secretHash, string title,
    string? description = null, DateTime? expiresOn = null,
    Dictionary<string, string>? customAttributes = null, IEnumerable<RoleAggregate>? roles = null) : base()
  {
    ApiKeyCreatedEvent e = new()
    {
      ActorId = actorId,
      RealmId = realm.Id,
      SecretHash = secretHash,
      Title = title.Trim(),
      Description = description?.CleanTrim(),
      ExpiresOn = expiresOn?.ToUniversalTime(),
      CustomAttributes = customAttributes ?? new(),
      Roles = roles?.Select(role => role.Id) ?? Enumerable.Empty<AggregateId>()
    };
    new ApiKeyCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

  /// <summary>
  /// Gets the identifier of the realm in which the API key belongs.
  /// </summary>
  public AggregateId RealmId { get; private set; }

  /// <summary>
  /// Gets the salted and hashed secret of the API key.
  /// </summary>
  public string SecretHash { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the title (or display name) of the API key.
  /// </summary>
  public string Title { get; private set; } = string.Empty;
  /// <summary>
  /// Gets a textual description for the API key.
  /// </summary>
  public string? Description { get; private set; }

  /// <summary>
  /// Gets the date and time when the API key expires.
  /// </summary>
  public DateTime? ExpiresOn { get; private set; }

  /// <summary>
  /// Gets the custom attributes of the API key.
  /// </summary>
  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  /// <summary>
  /// Gets the role (scope) identifiers of the API key.
  /// </summary>
  public IReadOnlyCollection<AggregateId> Roles => _roles.AsReadOnly();

  /// <summary>
  /// Returns a value indicating whether or not the API key is expired.
  /// </summary>
  /// <param name="moment">The date and time to compare to the expiration date and time.</param>
  /// <returns>True if the API key is expired.</returns>
  public bool IsExpired(DateTime? moment = null) => ExpiresOn <= (moment ?? DateTime.UtcNow);

  /// <summary>
  /// Applies the specified event to the API key.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(ApiKeyCreatedEvent e)
  {
    RealmId = e.RealmId;

    SecretHash = e.SecretHash;

    Title = e.Title;
    Description = e.Description;

    ExpiresOn = e.ExpiresOn;

    _customAttributes.Clear();
    _customAttributes.AddRange(e.CustomAttributes);

    _roles.Clear();
    _roles.AddRange(e.Roles);
  }

  /// <summary>
  /// Deletes the API key.
  /// </summary>
  /// <param name="actorId">The identifier of the actor deleting the API key.</param>
  public void Delete(AggregateId actorId)
  {
    ApplyChange(new ApiKeyDeletedEvent
    {
      ActorId = actorId
    });
  }
  /// <summary>
  /// Applies the specified event to the API key.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(ApiKeyDeletedEvent e)
  {
  }

  /// <summary>
  /// Updates the API key using the specified arguments.
  /// </summary>
  /// <param name="actorId">The identifier of the actor creating the API key.</param>
  /// <param name="title">The title (or display name) of the API key.</param>
  /// <param name="description">A textual description for the API key.</param>
  /// <param name="customAttributes">The custom attributes of the API key.</param>
  /// <param name="roles">The roles (or scopes) of the API key.</param>
  public void Update(AggregateId actorId, string title, string? description,
    Dictionary<string, string>? customAttributes, IEnumerable<RoleAggregate>? roles)
  {
    ApiKeyUpdatedEvent e = new()
    {
      ActorId = actorId,
      Title = title.Trim(),
      Description = description?.CleanTrim(),
      CustomAttributes = customAttributes ?? new(),
      Roles = roles?.Select(role => role.Id) ?? Enumerable.Empty<AggregateId>()
    };
    new ApiKeyUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  /// <summary>
  /// Applies the specified event to the API key.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(ApiKeyUpdatedEvent e)
  {
    Title = e.Title;
    Description = e.Description;

    _customAttributes.Clear();
    _customAttributes.AddRange(e.CustomAttributes);
  }

  /// <summary>
  /// Returns a string representation of the current API key.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => $"{Title} | {base.ToString()}";
}
