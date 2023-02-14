using System.Text;

namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when an aggregate is missing an Apply method for a domain event.
/// </summary>
public class EventNotSupportedException : NotSupportedException
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EventNotSupportedException"/> class with the specifies types.
  /// </summary>
  /// <param name="aggregateType">The type of the aggregate missing the Apply method</param>
  /// <param name="eventType">The type of the event for which the Apply method is missing</param>
  public EventNotSupportedException(Type aggregateType, Type eventType) : base(GetMessage(aggregateType, eventType))
  {
    Data["AggregateType"] = aggregateType.GetName();
    Data["EventType"] = eventType.GetName();
  }

  /// <summary>
  /// Builds the exception message using the specified types.
  /// </summary>
  /// <param name="aggregateType">The type of the aggregate missing the Apply method</param>
  /// <param name="eventType">The type of the event for which the Apply method is missing</param>
  /// <returns>The exception message</returns>
  private static string GetMessage(Type aggregateType, Type eventType)
  {
    StringBuilder message = new();

    message.AppendLine("The specified aggregate type does not provide an Apply method with the specified event type as its only argument.");
    message.AppendLine($"Aggregate type: {aggregateType.GetName()}");
    message.AppendLine($"Event type: {eventType.GetName()}");

    return message.ToString();
  }
}
