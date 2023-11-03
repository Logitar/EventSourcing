namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when an aggregate is missing its identifier public constructor.
/// </summary>
public class MissingAggregateConstructorException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MissingAggregateConstructorException"/> class.
  /// </summary>
  /// <param name="type">The type of the aggregate.</param>
  /// <exception cref="ArgumentOutOfRangeException">The specified type is not a subclass of the <see cref="AggregateRoot"/> type.</exception>
  public MissingAggregateConstructorException(Type type) : base(BuildMessage(type))
  {
    if (!type.IsSubclassOf(typeof(AggregateRoot)))
    {
      throw new ArgumentOutOfRangeException(nameof(type), $"The type must be a subclass of the '{nameof(AggregateRoot)}' type.");
    }

    AggregateType = type.GetName();
  }

  /// <summary>
  /// Gets or sets the type of the aggregate.
  /// </summary>
  public string AggregateType
  {
    get => (string)Data[nameof(AggregateType)]!;
    private set => Data[nameof(AggregateType)] = value;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="type">The type of the aggregate.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(Type type)
  {
    StringBuilder message = new();

    message.AppendLine("The specified aggregate type does not declare a public constructor receiving an AggregateId as its only argument.");
    message.Append("TypeName: ").AppendLine(type.GetName());

    return message.ToString();
  }
}

/// <summary>
/// The typed exception thrown when an aggregate is missing its identifier public constructor.
/// </summary>
/// <typeparam name="T">The type of the aggregate.</typeparam>
public class MissingAggregateConstructorException<T> : MissingAggregateConstructorException where T : AggregateRoot
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MissingAggregateConstructorException{T}"/> class.
  /// </summary>
  public MissingAggregateConstructorException() : base(typeof(T))
  {
  }
}
