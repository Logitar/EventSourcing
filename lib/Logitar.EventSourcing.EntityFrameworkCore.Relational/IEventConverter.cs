using Logitar.EventSourcing.EntityFrameworkCore.Relational.Entities;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

public interface IEventConverter
{
  EventEntity ToEventEntity(IEvent @event, StreamEntity stream);
}
