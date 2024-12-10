namespace Logitar.EventSourcing;

public interface IDeletableAggregate : IAggregate
{
  bool IsDeleted { get; }
}
