namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class ListExtensionsTests
{
  [Fact]
  public void When_adding_a_not_null_item_Then_it_is_added()
  {
    List<TestAggregate> aggregates = new();
    Assert.Empty(aggregates);

    aggregates.Add(new TestAggregate());
    Assert.NotEmpty(aggregates);
  }

  [Fact]
  public void When_adding_a_null_item_Then_it_is_not_added()
  {
    List<TestAggregate> aggregates = new();
    Assert.Empty(aggregates);

    aggregates.AddIfNotNull(null!);
    Assert.Empty(aggregates);
  }
}
