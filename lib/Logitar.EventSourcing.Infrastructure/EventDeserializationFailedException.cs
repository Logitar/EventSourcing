namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// The exception thrown when the deserialization of an event failed, returning null.
/// </summary>
public class EventDeserializationFailedException : Exception
{
  /// <summary>
  /// The detailed error message.
  /// </summary>
  private const string ErrorMessage = "The deserialization of the specified event failed.";

  /// <summary>
  /// Gets or sets the type of the event.
  /// </summary>
  public string Type
  {
    get => (string)Data[nameof(Type)]!;
    private set => Data[nameof(Type)] = value;
  }
  /// <summary>
  /// Gets or sets the string representation of the event.
  /// </summary>
  public string Value
  {
    get => (string)Data[nameof(Value)]!;
    private set => Data[nameof(Value)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventDeserializationFailedException"/> class.
  /// </summary>
  /// <param name="type">The type of the event.</param>
  /// <param name="value">The string representation of the event.</param>
  public EventDeserializationFailedException(Type type, string value) : base(BuildMessage(type, value))
  {
    Type = type.GetNamespaceQualifiedName();
    Value = value;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="type">The type of the event.</param>
  /// <param name="value">The string representation of the event.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(Type type, string value) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Type), type.GetNamespaceQualifiedName())
    .AddData(nameof(Value), value)
    .Build();
}
