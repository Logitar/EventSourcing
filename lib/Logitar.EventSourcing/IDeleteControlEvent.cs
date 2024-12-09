namespace Logitar.EventSourcing;

public interface IDeleteControlEvent : IEvent
{
  bool? IsDeleted { get; }
}
