using System.Text;

namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when an event is applied to the wrong aggregate.
/// </summary>
public class EventAggregateMismatchException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EventAggregateMismatchException"/> class with the specified aggregate and event.
  /// </summary>
  /// <param name="aggregate">The aggregate unto which the event was applied</param>
  /// <param name="change">The event belonging to another aggregate</param>
  public EventAggregateMismatchException(AggregateRoot aggregate, DomainEvent change) : base(GetMessage(aggregate, change))
  {
    Data["Aggregate"] = aggregate.ToString();
    Data["AggregateId"] = aggregate.Id.ToString();
    Data["Event"] = change.ToString();
    Data["EventId"] = change.Id;
    Data["EventAggregateId"] = change.AggregateId.ToString();
  }

  /// <summary>
  /// Builds the exception message using the specified aggregate and event
  /// </summary>
  /// <param name="aggregate">The aggregate unto which the event was applied</param>
  /// <param name="change">The event belonging to another aggregate</param>
  /// <returns>The exception message</returns>
  private static string GetMessage(AggregateRoot aggregate, DomainEvent change)
  {
    StringBuilder message = new();

    message.AppendLine("The specified event does not belong to the specified aggregate.");
    message.AppendLine($"Aggregate: {aggregate}");
    message.AppendLine($"AggregateId: {aggregate.Id}");
    message.AppendLine($"Event: {change}");
    message.AppendLine($"EventId: {change.Id}");
    message.AppendLine($"EventAggregateId: {change.AggregateId}");

    return message.ToString();
  }
}
