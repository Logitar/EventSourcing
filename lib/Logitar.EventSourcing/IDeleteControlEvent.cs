namespace Logitar.EventSourcing;

/// <summary>
/// TODO(fpion): document
/// </summary>
public interface IDeleteControlEvent : IEvent
{
  /// <summary>
  /// TODO(fpion): document
  /// </summary>
  bool? IsDeleted { get; }
}
