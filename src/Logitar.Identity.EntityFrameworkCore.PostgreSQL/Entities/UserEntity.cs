using Logitar.EventSourcing;
using Logitar.Identity.Users.Events;
using System.Text.Json;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model representing an user.
/// </summary>
internal class UserEntity : AggregateEntity
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UserEntity"/> class using the specified arguments.
  /// </summary>
  /// <param name="e">The creation event.</param>
  /// <param name="realm">The realm the user belongs to.</param>
  public UserEntity(UserCreatedEvent e, RealmEntity realm) : base(e)
  {
    Realm = realm;
    RealmId = realm.RealmId;

    Username = e.Username;
    SetPassword(e, e.PasswordHash);

    FirstName = e.FirstName;
    MiddleName = e.MiddleName;
    LastName = e.LastName;
    FullName = e.FullName;
    Nickname = e.Nickname;

    Birthdate = e.Birthdate;
    Gender = e.Gender?.Value;

    Locale = e.Locale?.Name;
    TimeZone = e.TimeZone;

    Picture = e.Picture;
    Profile = e.Profile;
    Website = e.Website;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="UserEntity"/> class.
  /// </summary>
  private UserEntity()
  {
  }

  /// <summary>
  /// Gets the primary identifier of the user.
  /// </summary>
  public int UserId { get; private set; }

  /// <summary>
  /// Gets the realm in which the user belongs.
  /// </summary>
  public RealmEntity? Realm { get; private set; }
  /// <summary>
  /// Gets the identifier of the realm in which the user belongs.
  /// </summary>
  public int RealmId { get; private set; }

  /// <summary>
  /// Gets the unique name of the user.
  /// </summary>
  public string Username { get; private set; } = string.Empty;
  /// <summary>
  /// Gets the normalized unique name of the user for unicity purposes.
  /// </summary>
  public string UsernameNormalized
  {
    get => Username.ToUpper();
    private set { }
  }
  /// <summary>
  /// Gets the salted and hashed password of the user.
  /// </summary>
  public string? PasswordHash { get; private set; }
  /// <summary>
  /// Gets the identifier of the actor who changed the user's password lastly.
  /// </summary>
  public string? PasswordChangedById { get; private set; }
  /// <summary>
  /// Gets the date and time the password changed lastly.
  /// </summary>
  public DateTime? PasswordChangedOn { get; private set; }
  /// <summary>
  /// Gets a value indicating whether or not the user has a password.
  /// </summary>
  public bool HasPassword { get; private set; }

  /// <summary>
  /// Gets the identifier of the actor who disabled the user account.
  /// </summary>
  public string? DisabledById { get; private set; }
  /// <summary>
  /// Gets the date and time when the user account was disabled.
  /// </summary>
  public DateTime? DisabledOn { get; private set; }
  /// <summary>
  /// Gets a value indicating whether or not the user account is disabled.
  /// </summary>
  public bool IsDisabled { get; private set; }

  /// <summary>
  /// Gets the date and time when the user signed-in lastly.
  /// </summary>
  public DateTime? SignedInOn { get; private set; }

  /// <summary>
  /// Gets the first name(s) or given name(s) of the user.
  /// </summary>
  public string? FirstName { get; private set; }
  /// <summary>
  /// Gets the middle name(s) of the user.
  /// </summary>
  public string? MiddleName { get; private set; }
  /// <summary>
  /// Gets the last name(s) or surname(s) of the user.
  /// </summary>
  public string? LastName { get; private set; }
  /// <summary>
  /// Gets the full name of the user.
  /// </summary>
  public string? FullName { get; private set; }
  /// <summary>
  /// Gets the nickname(s) or casual name(s) or the user.
  /// </summary>
  public string? Nickname { get; private set; }

  /// <summary>
  /// Gets the birtdate of the user.
  /// </summary>
  public DateTime? Birthdate { get; private set; }
  /// <summary>
  /// Gets the gender of the user.
  /// </summary>
  public string? Gender { get; private set; }

  /// <summary>
  /// Gets the locale of the user.
  /// </summary>
  public string? Locale { get; private set; }
  /// <summary>
  /// Gets the time zone of the user. It should match the name of a time zone in the tz database.
  /// </summary>
  public string? TimeZone { get; private set; }

  /// <summary>
  /// Gets a link (URL) to the picture of the user.
  /// </summary>
  public string? Picture { get; private set; }
  /// <summary>
  /// Gets a link (URL) to the profile of the user.
  /// </summary>
  public string? Profile { get; private set; }
  /// <summary>
  /// Gets a link (URL) to the website of the user.
  /// </summary>
  public string? Website { get; private set; }

  /// <summary>
  /// Gets the custom attributes of the user.
  /// </summary>
  public string? CustomAttributes { get; private set; }

  /// <summary>
  /// Gets the list of external identifiers of the user.
  /// </summary>
  public List<ExternalIdentifierEntity> ExternalIdentifiers { get; private set; } = new();
  /// <summary>
  /// Gets the list of roles of the user.
  /// </summary>
  public List<RoleEntity> Roles { get; private set; } = new();

  /// <summary>
  /// Disables the user account to the state of the specified event.
  /// </summary>
  /// <param name="e">The disable event.</param>
  public void Disable(UserDisabledEvent e)
  {
    SetVersion(e);

    DisabledById = e.ActorId.Value;
    DisabledOn = e.OccurredOn;
    IsDisabled = true;
  }

  /// <summary>
  /// Enables the user account to the state of the specified event.
  /// </summary>
  /// <param name="e">The enable event.</param>
  public void Enable(UserEnabledEvent e)
  {
    Update(e);

    DisabledById = null;
    DisabledOn = null;
    IsDisabled = false;
  }

  /// <summary>
  /// Adds, removes or updates an external identifier of the user.
  /// </summary>
  /// <param name="e"></param>
  public void SaveExternalIdentifier(ExternalIdentifierSavedEvent e)
  {
    ExternalIdentifierEntity? externalIdentifier = ExternalIdentifiers.SingleOrDefault(x => x.Key == e.Key);

    if (e.Value == null)
    {
      if (externalIdentifier != null)
      {
        ExternalIdentifiers.Remove(externalIdentifier);
      }
    }
    else if (externalIdentifier == null)
    {
      externalIdentifier = new ExternalIdentifierEntity(e, this);
      ExternalIdentifiers.Add(externalIdentifier);
    }
    else
    {
      externalIdentifier.Update(e);
    }
  }

  /// <summary>
  /// Updates the user to the state of the specified event.
  /// </summary>
  /// <param name="e">The update event.</param>
  public void Update(UserUpdatedEvent e)
  {
    base.Update(e);

    SetPassword(e, e.PasswordHash);

    FirstName = e.FirstName;
    MiddleName = e.MiddleName;
    LastName = e.LastName;
    FullName = e.FullName;
    Nickname = e.Nickname;

    Birthdate = e.Birthdate;
    Gender = e.Gender?.Value;

    Locale = e.Locale?.Name;
    TimeZone = e.TimeZone;

    Picture = e.Picture;
    Profile = e.Profile;
    Website = e.Website;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }

  /// <summary>
  /// Sets the password the user.
  /// </summary>
  /// <param name="e">The password change event.</param>
  /// <param name="passwordHash">The new password the user.</param>
  private void SetPassword(DomainEvent e, string? passwordHash)
  {
    if (passwordHash != null)
    {
      PasswordHash = passwordHash;
      PasswordChangedById = e.ActorId.Value;
      PasswordChangedOn = e.OccurredOn;
      HasPassword = true;
    }
  }
}
