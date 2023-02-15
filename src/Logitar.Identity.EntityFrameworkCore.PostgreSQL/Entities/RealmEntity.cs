using Logitar.Identity.Realms.Events;
using System.Text.Json;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model representing a realm.
/// </summary>
internal class RealmEntity : AggregateEntity
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RealmEntity"/> to the state of the specified event.
  /// </summary>
  /// <param name="e">The creation event.</param>
  public RealmEntity(RealmCreatedEvent e) : base(e)
  {
    UniqueName = e.UniqueName;
    DisplayName = e.DisplayName;
    Description = e.Description;

    DefaultLocale = e.DefaultLocale?.Name;
    Url = e.Url;

    RequireConfirmedAccount = e.RequireConfirmedAccount;
    RequireUniqueEmail = e.RequireUniqueEmail;

    UsernameSettings = JsonSerializer.Serialize(e.UsernameSettings);
    PasswordSettings = JsonSerializer.Serialize(e.PasswordSettings);

    JwtSecret = e.JwtSecret;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;

    GoogleOAuth2Configuration = e.GoogleOAuth2Configuration == null ? null : JsonSerializer.Serialize(e.GoogleOAuth2Configuration);
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="RealmEntity"/> class.
  /// </summary>
  private RealmEntity() : base()
  {
  }

  /// <summary>
  /// Gets the primary identifier of the realm.
  /// </summary>
  public int RealmId { get; private set; }

  /// <summary>
  /// Gets the unique name of the realm (not case-sensitive).
  /// </summary>
  public string UniqueName { get; private set; } = string.Empty;
  /// <summary>
  /// Gets the normalized unique name of the realm (case-sensitive).
  /// </summary>
  public string UniqueNameNormalized
  {
    get => UniqueName.ToUpper();
    private set { }
  }
  /// <summary>
  /// Gets the display name of the realm.
  /// </summary>
  public string? DisplayName { get; private set; }
  /// <summary>
  /// Gets a textual description for the realm.
  /// </summary>
  public string? Description { get; private set; }

  /// <summary>
  /// Gets the default locale of the realm.
  /// </summary>
  public string? DefaultLocale { get; private set; }
  /// <summary>
  /// Gets the URL of the realm, if it is used by an external Web application.
  /// </summary>
  public string? Url { get; private set; }

  /// <summary>
  /// If true, a confirmed contact is required for the user to be able to sign-in.
  /// </summary>
  public bool RequireConfirmedAccount { get; private set; }
  /// <summary>
  /// If true, primary email addresses unicity will be enforced in this realm, allowing users to log
  /// in with either their username and their primary email address.
  /// </summary>
  public bool RequireUniqueEmail { get; private set; }

  /// <summary>
  /// Gets the serialized settings used to validate usernames in this realm.
  /// </summary>
  public string UsernameSettings { get; private set; } = string.Empty;
  /// <summary>
  /// Gets the serialized settings used to validate passwords in this realm.
  /// </summary>
  public string PasswordSettings { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the secret used to sign JSON Web Tokens.
  /// </summary>
  public string JwtSecret { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the custom attributes of the realm.
  /// </summary>
  public string? CustomAttributes { get; private set; }

  /// <summary>
  /// Gets the Google OAuth 2.0 provider authentication configuration.
  /// </summary>
  public string? GoogleOAuth2Configuration { get; private set; }

  /// <summary>
  /// Gets the list of roles in this realm.
  /// </summary>
  public List<RoleEntity> Roles { get; private set; } = new();

  /// <summary>
  /// Updates the realm to the state of the specified event.
  /// </summary>
  /// <param name="e">The update event.</param>
  public void Update(RealmUpdatedEvent e)
  {
    base.Update(e);

    DisplayName = e.DisplayName;
    Description = e.Description;

    DefaultLocale = e.DefaultLocale?.Name;
    Url = e.Url;

    RequireConfirmedAccount = e.RequireConfirmedAccount;
    RequireUniqueEmail = e.RequireUniqueEmail;

    UsernameSettings = JsonSerializer.Serialize(e.UsernameSettings);
    PasswordSettings = JsonSerializer.Serialize(e.PasswordSettings);

    JwtSecret = e.JwtSecret;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;

    GoogleOAuth2Configuration = e.GoogleOAuth2Configuration == null ? null : JsonSerializer.Serialize(e.GoogleOAuth2Configuration);
  }
}
