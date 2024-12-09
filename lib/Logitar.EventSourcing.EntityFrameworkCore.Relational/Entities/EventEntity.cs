namespace Logitar.EventSourcing.EntityFrameworkCore.Relational.Entities;

public class EventEntity
{
  public long EventId { get; private set; }
  public string Id { get; private set; } = string.Empty;

  public StreamEntity? Stream { get; private set; }
  public long StreamId { get; private set; }
  public long Version { get; private set; }

  public string? ActorId { get; private set; }
  public DateTime OccurredOn { get; private set; }

  public bool? IsDeleted { get; private set; }

  public string TypeName { get; private set; } = string.Empty;
  public string NamespacedType { get; private set; } = string.Empty;
  public string Data { get; private set; } = string.Empty;

  public EventEntity(EventId id, StreamEntity stream, long? version, ActorId? actorId, DateTime? occurredOn, bool? isDeleted, string typeName, string namespacedType, string data)
  {
    Id = id.Value;

    Stream = stream;
    StreamId = stream.StreamId;
    Version = version ?? (stream.Version + 1);

    ActorId = actorId?.Value;
    OccurredOn = (occurredOn ?? DateTime.Now).AsUniversalTime();

    IsDeleted = isDeleted;

    TypeName = typeName;
    NamespacedType = namespacedType;
    Data = data;
  }

  private EventEntity()
  {
  }

  public override bool Equals(object? obj) => obj is EventEntity @event && @event.Id != Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{TypeName} | {GetType()} (Id={Id})";
}
