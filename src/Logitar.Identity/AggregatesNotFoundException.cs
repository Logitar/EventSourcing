using Logitar.EventSourcing;
using System.Text;

namespace Logitar.Identity;

/// <summary>
/// The exception thrown when a list of aggregates could not be found.
/// </summary>
public class AggregatesNotFoundException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregatesNotFoundException"/> class using the specified arguments.
  /// </summary>
  /// <param name="type">The type of the aggregates.</param>
  /// <param name="id">The identifiers of the aggregates.</param>
  /// <param name="paramName">The name of the identifiers parameter.</param>
  public AggregatesNotFoundException(Type type, IEnumerable<AggregateId> ids, string? paramName = null)
    : base(GetMessage(type, ids, paramName))
  {
    Data["Type"] = type.GetName();
    Data["Ids"] = ids;

    if (paramName != null)
    {
      Data["ParamName"] = paramName;
    }
  }

  /// <summary>
  /// Builds the exception message using the specified arguments.
  /// </summary>
  /// <param name="type">The type of the aggregates.</param>
  /// <param name="id">The identifiers of the aggregates.</param>
  /// <param name="paramName">The name of the identifiers parameter.</param>
  /// <returns>The exception message.</returns>
  private static string GetMessage(Type type, IEnumerable<AggregateId> ids, string? paramName)
  {
    StringBuilder message = new();

    message.AppendLine("The specified aggregate could not be found.");
    message.AppendLine($"Type: {type.GetName()}");
    message.AppendLine("Ids:");

    foreach (AggregateId id in ids)
    {
      message.AppendLine($" - {id}");
    }

    if (paramName != null)
    {
      message.AppendLine($"ParamName: {paramName}");
    }

    return message.ToString();
  }
}

/// <summary>
/// The typed exception thrown when a list of aggregates could not be found.
/// </summary>
/// <typeparam name="T">The type of the aggregates.</typeparam>
public class AggregatesNotFoundException<T> : AggregatesNotFoundException where T : AggregateRoot
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregatesNotFoundException{T}"/> class using the specified arguments.
  /// </summary>
  /// <param name="ids">The identifiers of the aggregates.</param>
  /// <param name="paramName">The name of the identifiers parameter.</param>
  public AggregatesNotFoundException(IEnumerable<AggregateId> ids, string? paramName = null) : base(typeof(T), ids, paramName)
  {
  }
}
