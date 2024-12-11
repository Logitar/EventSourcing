using Logitar.Data;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// Represents the event sourcing database.
/// </summary>
public static class EventDb
{
  /// <summary>
  /// The event sourcing database schema.
  /// </summary>
  public const string Schema = "EventSourcing";

  /// <summary>
  /// Represents the Events table.
  /// </summary>
  public static class Events
  {
    /// <summary>
    /// Gets the events table.
    /// </summary>
    public static readonly TableId Table = new(Schema, nameof(EventContext.Events), alias: null);

    /// <summary>
    /// Gets the ActorId column of the Events table.
    /// </summary>
    public static readonly ColumnId ActorId = new(nameof(EventEntity.ActorId), Table);
    /// <summary>
    /// Gets the Data column of the Events table.
    /// </summary>
    public static readonly ColumnId Data = new(nameof(EventEntity.Data), Table);
    /// <summary>
    /// Gets the EventId column of the Events table.
    /// </summary>
    public static readonly ColumnId EventId = new(nameof(EventEntity.EventId), Table);
    /// <summary>
    /// Gets the Id column of the Events table.
    /// </summary>
    public static readonly ColumnId Id = new(nameof(EventEntity.Id), Table);
    /// <summary>
    /// Gets the IsDeleted column of the Events table.
    /// </summary>
    public static readonly ColumnId IsDeleted = new(nameof(EventEntity.IsDeleted), Table);
    /// <summary>
    /// Gets the NamespacedType column of the Events table.
    /// </summary>
    public static readonly ColumnId NamespacedType = new(nameof(EventEntity.NamespacedType), Table);
    /// <summary>
    /// Gets the OccurredOn column of the Events table.
    /// </summary>
    public static readonly ColumnId OccurredOn = new(nameof(EventEntity.OccurredOn), Table);
    /// <summary>
    /// Gets the StreamId column of the Events table.
    /// </summary>
    public static readonly ColumnId StreamId = new(nameof(EventEntity.StreamId), Table);
    /// <summary>
    /// Gets the TypeName column of the Events table.
    /// </summary>
    public static readonly ColumnId TypeName = new(nameof(EventEntity.TypeName), Table);
    /// <summary>
    /// Gets the Version column of the Events table.
    /// </summary>
    public static readonly ColumnId Version = new(nameof(EventEntity.Version), Table);
  }

  /// <summary>
  /// Represents the Streams table.
  /// </summary>
  public static class Streams
  {
    /// <summary>
    /// Gets the streams table.
    /// </summary>
    public static readonly TableId Table = new(Schema, nameof(EventContext.Streams), alias: null);

    /// <summary>
    /// Gets the CreatedBy column of the Streams table.
    /// </summary>
    public static readonly ColumnId CreatedBy = new(nameof(StreamEntity.CreatedBy), Table);
    /// <summary>
    /// Gets the CreatedOn column of the Streams table.
    /// </summary>
    public static readonly ColumnId CreatedOn = new(nameof(StreamEntity.CreatedOn), Table);
    /// <summary>
    /// Gets the Id column of the Streams table.
    /// </summary>
    public static readonly ColumnId Id = new(nameof(StreamEntity.Id), Table);
    /// <summary>
    /// Gets the IsDeleted column of the Streams table.
    /// </summary>
    public static readonly ColumnId IsDeleted = new(nameof(StreamEntity.IsDeleted), Table);
    /// <summary>
    /// Gets the StreamId column of the Streams table.
    /// </summary>
    public static readonly ColumnId StreamId = new(nameof(StreamEntity.StreamId), Table);
    /// <summary>
    /// Gets the Type column of the Streams table.
    /// </summary>
    public static readonly ColumnId Type = new(nameof(StreamEntity.Type), Table);
    /// <summary>
    /// Gets the UpdatedBy column of the Streams table.
    /// </summary>
    public static readonly ColumnId UpdatedBy = new(nameof(StreamEntity.UpdatedBy), Table);
    /// <summary>
    /// Gets the UpdatedOn column of the Streams table.
    /// </summary>
    public static readonly ColumnId UpdatedOn = new(nameof(StreamEntity.UpdatedOn), Table);
    /// <summary>
    /// Gets the Version column of the Streams table.
    /// </summary>
    public static readonly ColumnId Version = new(nameof(StreamEntity.Version), Table);
  }
}
