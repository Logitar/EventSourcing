using System.Text;

namespace Logitar.EventSourcing;

public class CannotApplyPastEventException : Exception
{
  public CannotApplyPastEventException(AggregateRoot aggregate, DomainEvent change) : base(GetMessage(aggregate, change))
  {
    Data["Aggregate"] = aggregate.ToString();
    Data["AggregateId"] = aggregate.Id.ToString();
    Data["AggregateVersion"] = aggregate.Version;
    Data["Event"] = change.ToString();
    Data["EventId"] = change.Id;
    Data["EventVersion"] = change.Version;
  }

  private static string GetMessage(AggregateRoot aggregate, DomainEvent change)
  {
    StringBuilder message = new();

    message.AppendLine("The specified event is past the current state of the specified aggregate.");
    message.AppendLine($"Aggregate: {aggregate}");
    message.AppendLine($"AggregateId: {aggregate.Id}");
    message.AppendLine($"AggregateVersion: {aggregate.Version}");
    message.AppendLine($"Event: {change}");
    message.AppendLine($"EventId: {change.Id}");
    message.AppendLine($"EventVersion: {change.Version}");

    return message.ToString();
  }
}
