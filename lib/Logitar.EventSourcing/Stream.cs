namespace Logitar.EventSourcing;

public class Stream
{
  public StreamId Id { get; }
  public Type? Type { get; }
  public long Version { get; }

  public ActorId? CreatedBy { get; private set; }
  public DateTime? CreatedOn { get; private set; }

  public ActorId? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }

  public bool IsDeleted { get; private set; }

  public IReadOnlyCollection<Event> Events { get; private set; }

  public Stream(StreamId id, Type? type = null, IEnumerable<Event>? events = null)
  {
    Id = id;
    Type = type;

    if (events == null)
    {
      Events = [];
    }
    else
    {
      Events = events.ToArray();

      foreach (Event @event in events)
      {
        Version = @event.Version;

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
      }
    }
  }

  public override bool Equals(object obj) => obj is Stream stream && stream.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => Type == null ? $"{GetType()} (Id={Id})" : $"{Type} | {GetType()} (Id={Id})";
}
