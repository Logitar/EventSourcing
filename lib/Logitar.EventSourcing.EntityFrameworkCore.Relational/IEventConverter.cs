namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// Represents a converter, used to convert between event abstractions and database entities.
/// </summary>
public interface IEventConverter
{
  /// <summary>
  /// Converts the specified event to the corresponding database entity.
  /// </summary>
  /// <param name="event">The event to convert.</param>
  /// <param name="stream">The stream in which the event belongs.</param>
  /// <returns>The resulting event entity.</returns>
  EventEntity ToEventEntity(IEvent @event, StreamEntity stream);
}
