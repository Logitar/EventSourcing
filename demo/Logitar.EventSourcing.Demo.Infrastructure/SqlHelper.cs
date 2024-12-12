using Logitar.Data;
using Logitar.EventSourcing.Demo.Application.Search;

namespace Logitar.EventSourcing.Demo.Infrastructure;

public abstract class SqlHelper : ISqlHelper
{
  public void ApplyTextSearch(IQueryBuilder query, TextSearch search, params ColumnId[] columns)
  {
    int termCount = search.Terms.Count;
    if (termCount > 0 && columns.Length > 0)
    {
      List<Condition> conditions = new(capacity: termCount);
      foreach (SearchTerm term in search.Terms)
      {
        if (!string.IsNullOrWhiteSpace(term.Value))
        {
          string pattern = term.Value.Trim();
          conditions.Add(columns.Length == 1
            ? new OperatorCondition(columns.Single(), CreateOperator(pattern))
            : new OrCondition(columns.Select(column => new OperatorCondition(column, CreateOperator(pattern))).ToArray()));
        }
      }

      if (conditions.Count > 0)
      {
        switch (search.Operator)
        {
          case SearchOperator.And:
            query.WhereAnd([.. conditions]);
            break;
          case SearchOperator.Or:
            query.WhereOr([.. conditions]);
            break;
        }
      }
    }
  }
  protected virtual ConditionalOperator CreateOperator(string pattern) => Operators.IsLike(pattern);

  public abstract IQueryBuilder QueryFrom(TableId table);
}
