using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// Represents a converter, used to convert between event abstractions and database entities.
/// </summary>
public class EventConverter : IEventConverter
{
  /// <summary>
  /// Gets the event serializer.
  /// </summary>
  protected IEventSerializer Serializer { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventConverter"/> class.
  /// </summary>
  /// <param name="serializer">The event serializer.</param>
  public EventConverter(IEventSerializer serializer)
  {
    Serializer = serializer;
  }

  /// <summary>
  /// Converts the specified event to the corresponding database entity.
  /// </summary>
  /// <param name="event">The event to convert.</param>
  /// <param name="stream">The stream in which the event belongs.</param>
  /// <returns>The resulting event entity.</returns>
  public EventEntity ToEventEntity(IEvent @event, StreamEntity stream)
  {
    EventId id = GetEventId(@event);
    DateTime occurredOn = GetOccurredOn(@event);
    string typeName = GetEventType(@event);
    string namespacedType = @event.GetType().GetNamespaceQualifiedName();
    string data = GetEventData(@event);
    ActorId? actorId = GetActorId(@event);
    bool? isDeleted = IsDeleted(@event);

    return new EventEntity(id, stream, occurredOn, typeName, namespacedType, data, actorId, isDeleted);
  }

  /// <summary>
  /// Returns the identifier of the specified event.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The event identifier.</returns>
  protected virtual EventId GetEventId(IEvent @event) => @event is IIdentifiableEvent identifiable ? identifiable.Id : EventId.NewId();

  /// <summary>
  /// Returns the date and time at which the event occurred.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The date and time.</returns>
  protected virtual DateTime GetOccurredOn(IEvent @event) => @event is ITemporalEvent temporal ? temporal.OccurredOn : DateTime.Now;

  /// <summary>
  /// Returns a string representation of the event type.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The event type represented as a string.</returns>
  protected virtual string GetEventType(IEvent @event) => @event.GetType().Name;

  /// <summary>
  /// Returns the event data as a string.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The event data.</returns>
  protected virtual string GetEventData(IEvent @event) => Serializer.Serialize(@event);

  /// <summary>
  /// Returns the identifier of the actor who raised the event.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The actor identifier.</returns>
  protected virtual ActorId? GetActorId(IEvent @event) => @event is IActorEvent actor ? actor.ActorId : null;

  /// <summary>
  /// Returns a value indicating whether the event deletes, undeletes or leaves unchanged its stream deletion status.
  /// </summary>
  /// <param name="event">The status.</param>
  /// <returns>The boolean value.</returns>
  protected virtual bool? IsDeleted(IEvent @event)
  {
    if (@event is IDeleteControlEvent control && control.IsDeleted.HasValue)
    {
      return control.IsDeleted.Value;
    }
    else if (@event is IDeleteEvent && @event is not IUndeleteEvent)
    {
      return true;
    }
    else if (@event is IUndeleteEvent && @event is not IDeleteEvent)
    {
      return false;
    }

    return null;
  }
}
