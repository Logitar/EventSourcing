namespace Logitar.EventSourcing.Demo.Settings;

internal record BasicAuthenticationSettings
{
  public const string SectionKey = "BasicAuthentication";

  public string? Username { get; set; }
  public string? Password { get; set; }

  public bool IsEnabled => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
}
