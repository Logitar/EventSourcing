using Logitar.Data;
using Logitar.Data.PostgreSQL;

namespace Logitar.EventSourcing.Demo.Infrastructure;

internal class PostgresHelper : SqlHelper
{
  public override IQueryBuilder QueryFrom(TableId table) => PostgresQueryBuilder.From(table);

  protected override ConditionalOperator CreateOperator(string pattern) => PostgresOperators.IsLikeInsensitive(pattern);
}
