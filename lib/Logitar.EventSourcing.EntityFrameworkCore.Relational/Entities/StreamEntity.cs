namespace Logitar.EventSourcing.EntityFrameworkCore.Relational.Entities;

public sealed class StreamEntity
{
  public long StreamId { get; private set; }
  public string Id { get; private set; } = string.Empty;

  public string? Type { get; private set; }
  public long Version { get; private set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public bool IsDeleted { get; private set; }

  public List<EventEntity> Events { get; private set; } = [];

  public StreamEntity(StreamId id, Type? type = null)
  {
    Id = id.Value;

    Type = type?.GetNamespaceQualifiedName();

    CreatedOn = UpdatedOn = DateTime.UtcNow;
  }

  private StreamEntity()
  {
  }

  public void Append(EventEntity @event)
  {
    if ((@event.Stream != null && !@event.Stream.Equals(this)) || @event.StreamId != StreamId)
    {
      throw new NotImplementedException();
    }
    if (@event.Version != (Version + 1))
    {
      throw new NotImplementedException();
    }

    Version++;

    if (Version <= 1)
    {
      CreatedBy = @event.ActorId;
      CreatedOn = @event.OccurredOn;
    }

    UpdatedBy = @event.ActorId;
    UpdatedOn = @event.OccurredOn;

    if (@event.IsDeleted.HasValue)
    {
      IsDeleted = @event.IsDeleted.Value;
    }

    Events.Add(@event);
  }

  public override bool Equals(object? obj) => obj is StreamEntity stream && stream.Id != Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => Type == null ? $"{GetType()} (Id={Id})" : $"{Type} | {GetType()} (Id={Id})";
}
