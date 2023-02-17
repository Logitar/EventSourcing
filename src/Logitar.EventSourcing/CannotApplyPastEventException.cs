using System.Text;

namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when a past event is applied to an aggregate of a future state.
/// </summary>
public class CannotApplyPastEventException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="CannotApplyPastEventException"/> class with the specified aggregate and past event.
  /// </summary>
  /// <param name="aggregate">The aggregate in a future state</param>
  /// <param name="change">The event of a past state</param>
  public CannotApplyPastEventException(AggregateRoot aggregate, DomainEvent change) : base(GetMessage(aggregate, change))
  {
    Data["Aggregate"] = aggregate.ToString();
    Data["AggregateId"] = aggregate.Id.ToString();
    Data["AggregateVersion"] = aggregate.Version;
    Data["Event"] = change.ToString();
    Data["EventId"] = change.Id;
    Data["EventVersion"] = change.Version;
  }

  /// <summary>
  /// Builds the exception message using the specified aggregate and past event.
  /// </summary>
  /// <param name="aggregate">The aggregate in a future state</param>
  /// <param name="change">The event of a past state</param>
  /// <returns>The exception message</returns>
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
