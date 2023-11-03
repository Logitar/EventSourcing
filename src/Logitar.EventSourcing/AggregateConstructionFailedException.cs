namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when the construction of an aggregate failed, or returned null.
/// </summary>
public class AggregateConstructionFailedException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateConstructionFailedException"/> class.
  /// </summary>
  /// <param name="type">The type of the aggregate.</param>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <exception cref="ArgumentOutOfRangeException">The specified type is not a subclass of the <see cref="AggregateRoot"/> type.</exception>
  public AggregateConstructionFailedException(Type type, AggregateId id)
    : base(BuildMessage(type, id))
  {
    if (!type.IsSubclassOf(typeof(AggregateRoot)))
    {
      throw new ArgumentOutOfRangeException(nameof(type), $"The type must be a subclass of the '{nameof(AggregateRoot)}' type.");
    }

    AggregateType = type.GetName();
    AggregateId = id.ToString();
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
  /// Gets or sets the identifier of the aggregate.
  /// </summary>
  public string AggregateId
  {
    get => (string)Data[nameof(AggregateId)]!;
    private set => Data[nameof(AggregateId)] = value;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="type">The type of the aggregate.</param>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(Type type, AggregateId id)
  {
    StringBuilder message = new();

    message.AppendLine("The aggregate construction failed.");
    message.Append("TypeName: ").AppendLine(type.GetName());
    message.Append("AggregateId: ").Append(id).AppendLine();

    return message.ToString();
  }
}

/// <summary>
/// The typed exception thrown when the construction of an aggregate failed, or returned null.
/// </summary>
/// <typeparam name="T">The type of the aggregate.</typeparam>
public class AggregateConstructionFailedException<T> : AggregateConstructionFailedException where T : AggregateRoot
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateConstructionFailedException{T}"/> class.
  /// </summary>
  /// <param name="id">Th identifier of the aggregate.</param>
  public AggregateConstructionFailedException(AggregateId id) : base(typeof(T), id)
  {
  }
}
