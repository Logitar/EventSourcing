using Logitar.Data;

namespace Logitar.EventSourcing.Relational;

/// <summary>
/// Represents the Events table.
/// </summary>
public static class Events
{
  /// <summary>
  /// Gets the events table.
  /// </summary>
  public static TableId Table => new("Events");

  /// <summary>
  /// Gets the ActorId column of the Events table.
  /// </summary>
  public static ColumnId ActorId => new(nameof(EventEntity.ActorId), Table);
  /// <summary>
  /// Gets the AggregateId column of the Events table.
  /// </summary>
  public static ColumnId AggregateId => new(nameof(EventEntity.AggregateId), Table);
  /// <summary>
  /// Gets the AggregateType column of the Events table.
  /// </summary>
  public static ColumnId AggregateType => new(nameof(EventEntity.AggregateType), Table);
  /// <summary>
  /// Gets the EventData column of the Events table.
  /// </summary>
  public static ColumnId EventData => new(nameof(EventEntity.EventData), Table);
  /// <summary>
  /// Gets the EventType column of the Events table.
  /// </summary>
  public static ColumnId EventType => new(nameof(EventEntity.EventType), Table);
  /// <summary>
  /// Gets the EventId column of the Events table.
  /// </summary>
  public static ColumnId EventId => new(nameof(EventEntity.EventId), Table);
  /// <summary>
  /// Gets the Id column of the Events table.
  /// </summary>
  public static ColumnId Id => new(nameof(EventEntity.Id), Table);
  /// <summary>
  /// Gets the IsDeleted column of the Events table.
  /// </summary>
  public static ColumnId IsDeleted => new(nameof(EventEntity.IsDeleted), Table);
  /// <summary>
  /// Gets the OccurredOn column of the Events table.
  /// </summary>
  public static ColumnId OccurredOn => new(nameof(EventEntity.OccurredOn), Table);
  /// <summary>
  /// Gets the Version column of the Events table.
  /// </summary>
  public static ColumnId Version => new(nameof(EventEntity.Version), Table);
}
