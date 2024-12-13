namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// The exception thrown when the type of an event stream could not be found.
/// </summary>
public class StreamTypeNotFoundException : Exception
{
  /// <summary>
  /// The detailed error message.
  /// </summary>
  private const string ErrorMessage = "The specified event stream type could not be found.";

  /// <summary>
  /// Gets or sets the identifier of the invalid stream.
  /// </summary>
  public string StreamId
  {
    get => (string)Data[nameof(StreamId)]!;
    private set => Data[nameof(StreamId)] = value;
  }
  /// <summary>
  /// Gets or sets the name of the type that could not be found.
  /// </summary>
  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamTypeNotFoundException"/> class.
  /// </summary>
  /// <param name="stream">The stream.</param>
  public StreamTypeNotFoundException(StreamEntity stream) : base(BuildMessage(stream))
  {
    if (stream.Type == null)
    {
      throw new ArgumentException($"The {nameof(stream.Type)} is required.", nameof(stream));
    }

    StreamId = stream.Id;
    TypeName = stream.Type;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="stream">The invalid stream.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(StreamEntity stream) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(StreamId), stream.Id)
    .AddData(nameof(TypeName), stream.Type)
    .Build();
}
