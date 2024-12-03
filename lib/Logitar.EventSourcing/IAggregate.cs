namespace Logitar.EventSourcing;

public interface IAggregate
{
  AggregateId Id { get; }
}
