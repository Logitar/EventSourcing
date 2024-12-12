using Logitar.Data;
using Logitar.EventSourcing.Demo.Application.Search;

namespace Logitar.EventSourcing.Demo.Infrastructure;

public interface ISqlHelper
{
  void ApplyTextSearch(IQueryBuilder query, TextSearch search, params ColumnId[] columns);

  IQueryBuilder QueryFrom(TableId table);
}
