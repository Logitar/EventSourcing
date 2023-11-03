namespace Logitar.EventSourcing.Infrastructure;

internal class EventEntityMock : IEventEntity
{
  public Guid Id { get; init; }

  public string EventType { get; init; } = string.Empty;
  public string EventData { get; init; } = string.Empty;
}
