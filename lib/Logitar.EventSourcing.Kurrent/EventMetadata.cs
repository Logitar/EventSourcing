namespace Logitar.EventSourcing.Kurrent;

public record EventMetadata(
  Type EventType,
  EventId? EventId,
  Type? StreamType,
  long? Version,
  ActorId? ActorId,
  DateTime? OccurredOn,
  bool? IsDeleted);
