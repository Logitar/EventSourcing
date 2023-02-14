namespace Logitar.EventSourcing;

/// <summary>
/// Represents the delete actions that can be performed on a domain aggregate.
/// </summary>
public enum DeleteAction
{
  /// <summary>
  /// Nothing will happen.
  /// </summary>
  None = 0,

  /// <summary>
  /// The aggregate will be deleted.
  /// </summary>
  Delete = 1,

  /// <summary>
  /// The aggregate will be restored.
  /// </summary>
  Undelete = 2
}
