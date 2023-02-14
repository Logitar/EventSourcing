namespace Logitar.EventSourcing;

public record AggregateDeleted : DomainEvent
{
  public AggregateDeleted()
  {
    DeleteAction = DeleteAction.Delete;
  }
}
