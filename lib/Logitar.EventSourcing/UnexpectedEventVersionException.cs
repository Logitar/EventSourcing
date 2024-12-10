namespace Logitar.EventSourcing;

public class UnexpectedEventVersionException : Exception
{
  private const string ErrorMessage = "The version of the specified version was not expected.";

  public string AggregateId
  {
    get => (string)Data[nameof(AggregateId)]!;
    private set => Data[nameof(AggregateId)] = value;
  }
  public long AggregateVersion
  {
    get => (long)Data[nameof(AggregateVersion)]!;
    private set => Data[nameof(AggregateVersion)] = value;
  }
  public string? EventId
  {
    get => (string?)Data[nameof(EventId)];
    private set => Data[nameof(EventId)] = value;
  }
  public long EventVersion
  {
    get => (long)Data[nameof(EventVersion)]!;
    private set => Data[nameof(EventVersion)] = value;
  }

  public UnexpectedEventVersionException(IVersionedAggregate aggregate, IVersionedEvent @event) : base(BuildMessage(aggregate, @event))
  {
    AggregateId = aggregate.Id.Value;
    AggregateVersion = aggregate.Version;
    EventId = @event is IIdentifiableEvent identifiable ? identifiable.Id.Value : null;
    EventVersion = @event.Version;
  }

  private static string BuildMessage(IVersionedAggregate aggregate, IVersionedEvent @event) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(AggregateId), aggregate.Id)
    .AddData(nameof(AggregateVersion), aggregate.Version)
    .AddData(nameof(EventId), @event is IIdentifiableEvent identifiable ? identifiable.Id : null, "<null>")
    .AddData(nameof(EventVersion), @event.Version)
    .Build();
}
