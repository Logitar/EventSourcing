namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the verification actions that can be performed on a contact aggregate.
/// </summary>
public enum VerificationAction
{
  /// <summary>
  /// The verification status will remain unchanged.
  /// </summary>
  None = 0,

  /// <summary>
  /// The contact will be verified.
  /// </summary>
  Verify = 1,

  /// <summary>
  /// The contact will be unverified.
  /// </summary>
  Unverify = 2
}
