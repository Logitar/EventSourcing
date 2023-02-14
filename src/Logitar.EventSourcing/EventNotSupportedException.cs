using System.Text;

namespace Logitar.EventSourcing;

public class EventNotSupportedException : NotSupportedException
{
  public EventNotSupportedException(Type aggregateType, Type eventType) : base(GetMessage(aggregateType, eventType))
  {
    Data["AggregateType"] = aggregateType.GetName();
    Data["EventType"] = eventType.GetName();
  }

  private static string GetMessage(Type aggregateType, Type eventType)
  {
    StringBuilder message = new();

    message.AppendLine("The specified aggregate type does not provide an Apply method with the specified event type as its only argument.");
    message.AppendLine($"Aggregate type: {aggregateType.GetName()}");
    message.AppendLine($"Event type: {eventType.GetName()}");

    return message.ToString();
  }
}
