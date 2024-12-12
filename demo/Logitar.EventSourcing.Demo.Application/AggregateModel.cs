namespace Logitar.EventSourcing.Demo.Application;

public class AggregateModel
{
  public string Id { get; set; } = string.Empty;
  public long Version { get; set; }

  public DateTime CreatedOn { get; set; }
  public DateTime UpdatedOn { get; set; }

  public override bool Equals(object? obj) => obj is AggregateModel aggregate && aggregate.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{GetType()} (Id={Id})";
}
