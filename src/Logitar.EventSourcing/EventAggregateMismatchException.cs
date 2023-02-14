using System.Text;

namespace Logitar.EventSourcing;

public class EventAggregateMismatchException : Exception
{
  public EventAggregateMismatchException(AggregateRoot aggregate, DomainEvent change) : base(GetMessage(aggregate, change))
  {
    Data["Aggregate"] = aggregate.ToString();
    Data["AggregateId"] = aggregate.Id.ToString();
    Data["Event"] = change.ToString();
    Data["EventId"] = change.Id;
    Data["EventAggregateId"] = change.AggregateId.ToString();
  }

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
