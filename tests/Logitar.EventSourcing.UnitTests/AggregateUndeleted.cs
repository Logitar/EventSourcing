namespace Logitar.EventSourcing;

public record AggregateUndeleted : DomainEvent
{
  public AggregateUndeleted()
  {
    DeleteAction = DeleteAction.Undelete;
  }
}
