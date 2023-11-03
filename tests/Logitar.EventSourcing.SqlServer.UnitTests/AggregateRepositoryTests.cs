using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Data.SqlClient;
using Moq;

namespace Logitar.EventSourcing.SqlServer;

[Trait(Traits.Category, Categories.Unit)]
public class AggregateRepositoryTests
{
  private readonly Mock<IEventBus> _eventBus = new();
  private readonly EventSerializer _eventSerializer = new();

  private readonly AggregateRepository _repository;

  public AggregateRepositoryTests()
  {
    SqlConnection connection = new();
    _repository = new(connection, _eventBus.Object, _eventSerializer);
  }

  [Fact(DisplayName = "From: it should return the correct query builder.")]
  public void From_it_should_return_the_correct_query_builder()
  {
    MethodInfo? fromMethod = _repository.GetType().GetMethod("From", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.NotNull(fromMethod);

    TableId source = new("Events");
    IQueryBuilder? builder = (IQueryBuilder?)fromMethod.Invoke(_repository, new[] { source });
    Assert.NotNull(builder);

    Assert.True(builder is SqlServerQueryBuilder);
  }
}
