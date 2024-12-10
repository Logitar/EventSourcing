namespace Logitar.EventSourcing;

public class StreamMismatchException : Exception
{
  private const string ErrorMessage = "The stream identifier of the specified event did not match the identifier of the specified stream.";

  public string AggregateStreamId
  {
    get => (string)Data[nameof(AggregateStreamId)]!;
    private set => Data[nameof(AggregateStreamId)] = value;
  }
  public string EventStreamId
  {
    get => (string)Data[nameof(EventStreamId)]!;
    private set => Data[nameof(EventStreamId)] = value;
  }
  public string? EventId
  {
    get => (string)Data[nameof(EventId)];
    private set => Data[nameof(EventId)] = value;
  }

  public StreamMismatchException(IAggregate aggregate, IStreamEvent @event) : base(BuildMessage(aggregate, @event))
  {
    AggregateStreamId = aggregate.Id.Value;
    EventStreamId = @event.StreamId.Value;

    if (@event is IIdentifiableEvent identifiable)
    {
      EventId = identifiable.Id.Value;
    }
  }

  private static string BuildMessage(IAggregate aggregate, IStreamEvent @event) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(AggregateStreamId), aggregate.Id)
    .AddData(nameof(EventStreamId), @event.StreamId)
    .AddData(nameof(EventId), @event is IIdentifiableEvent identifiable ? identifiable.Id : null, "<null>")
    .Build();
}
