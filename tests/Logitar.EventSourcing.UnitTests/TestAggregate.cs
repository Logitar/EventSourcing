namespace Logitar.EventSourcing;

public class TestAggregate : AggregateRoot
{
  public TestAggregate() : base()
  {
  }
  public TestAggregate(AggregateId id) : base(id)
  {
  }

  public string Name { get; private set; } = string.Empty;

  public void Delete() => ApplyChange(new AggregateDeleted());
  protected virtual void Apply(AggregateDeleted e)
  {
  }

  public void Fail() => ApplyChange(new AggregateFailed());

  public void Rename(string name) => ApplyChange(new AggregateRenamed(name));
  protected virtual void Apply(AggregateRenamed e)
  {
    Name = e.Name;
  }

  public void Undelete() => ApplyChange(new AggregateUndeleted());
  protected virtual void Apply(AggregateUndeleted e)
  {
  }
}
