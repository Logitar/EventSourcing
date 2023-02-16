using Logitar.EventSourcing;
using System.Text;

namespace Logitar.Identity;

/// <summary>
/// The exception thrown when an aggregate could not be found.
/// </summary>
public class AggregateNotFoundException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateNotFoundException"/> class using the specified arguments.
  /// </summary>
  /// <param name="type">The type of the aggregate.</param>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="paramName">The name of the identifier parameter.</param>
  public AggregateNotFoundException(Type type, AggregateId id, string? paramName = null)
    : base(GetMessage(type, id, paramName))
  {
    Data["Type"] = type.GetName();
    Data["Id"] = id;

    if (paramName != null)
    {
      Data["ParamName"] = paramName;
    }
  }

  /// <summary>
  /// Builds the exception message using the specified arguments.
  /// </summary>
  /// <param name="type">The type of the aggregate.</param>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="paramName">The name of the identifier parameter.</param>
  /// <returns>The exception message.</returns>
  private static string GetMessage(Type type, AggregateId id, string? paramName)
  {
    StringBuilder message = new();

    message.AppendLine("The specified aggregate could not be found.");
    message.AppendLine($"Type: {type.GetName()}");
    message.AppendLine($"Id: {id}");

    if (paramName != null)
    {
      message.AppendLine($"ParamName: {paramName}");
    }

    return message.ToString();
  }
}

/// <summary>
/// The typed exception thrown when an aggregate could not be found.
/// </summary>
/// <typeparam name="T">The type of the aggregate.</typeparam>
public class AggregateNotFoundException<T> : AggregateNotFoundException where T : AggregateRoot
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateNotFoundException{T}"/> class using the specified arguments.
  /// </summary>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="paramName">The name of the identifier parameter.</param>
  public AggregateNotFoundException(AggregateId id, string? paramName = null) : base(typeof(T), id, paramName)
  {
  }
}
