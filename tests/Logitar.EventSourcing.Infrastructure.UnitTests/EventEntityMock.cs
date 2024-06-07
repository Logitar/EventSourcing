namespace Logitar.EventSourcing.Infrastructure;

internal class EventEntityMock : IEventEntity
{
  public string Id { get; init; } = string.Empty;

  public string EventType { get; init; } = string.Empty;
  public string EventData { get; init; } = string.Empty;
}
