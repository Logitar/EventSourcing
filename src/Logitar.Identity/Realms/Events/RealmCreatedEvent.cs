using Logitar.EventSourcing;
using Logitar.Identity.Accounts;
using MediatR;
using System.Globalization;

namespace Logitar.Identity.Realms.Events;

/// <summary>
/// Represents the event raised when a new <see cref="RealmAggregate"/> is created.
/// </summary>
public record RealmCreatedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Gets or sets the unique name of the realm (not case-sensitive).
  /// </summary>
  public string UniqueName { get; init; } = string.Empty;
  /// <summary>
  /// Gets or sets the display name of the realm.
  /// </summary>
  public string? DisplayName { get; init; }
  /// <summary>
  /// Gets or sets a textual description for the realm.
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Gets or sets the default locale of the realm.
  /// </summary>
  public CultureInfo? DefaultLocale { get; init; }
  /// <summary>
  /// Gets or sets the URL of the realm, if it is used by an external Web application.
  /// </summary>
  public string? Url { get; init; }

  /// <summary>
  /// If true, a confirmed contact is required for the user to be able to sign-in.
  /// </summary>
  public bool RequireConfirmedAccount { get; init; }
  /// <summary>
  /// If true, primary email addresses unicity will be enforced in this realm, allowing users to log
  /// in with either their username and their primary email address.
  /// </summary>
  public bool RequireUniqueEmail { get; init; }

  /// <summary>
  /// Gets or sets the settings used to validate usernames in this realm.
  /// </summary>
  public ReadOnlyUsernameSettings UsernameSettings { get; init; } = new();
  /// <summary>
  /// Gets or sets the settings used to validate passwords in this realm.
  /// </summary>
  public ReadOnlyPasswordSettings PasswordSettings { get; init; } = new();

  /// <summary>
  /// Gets or sets the secret used to sign JSON Web Tokens.
  /// </summary>
  public string JwtSecret { get; init; } = string.Empty;

  /// <summary>
  /// Gets or sets the external authentication provider configurations of the realm.
  /// </summary>
  public Dictionary<ExternalProvider, ExternalProviderConfiguration> ExternalProviders { get; init; } = new();

  /// <summary>
  /// Gets or sets the custom attributes of the realm.
  /// </summary>
  public Dictionary<string, string> CustomAttributes { get; init; } = new();
}
