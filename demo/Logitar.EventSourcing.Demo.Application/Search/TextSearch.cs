namespace Logitar.EventSourcing.Demo.Application.Search;

public record TextSearch
{
  public List<SearchTerm> Terms { get; set; } = [];
  public SearchOperator Operator { get; set; } = SearchOperator.And;

  public TextSearch()
  {
  }

  public TextSearch(IEnumerable<SearchTerm> terms, SearchOperator @operator = SearchOperator.And)
  {
    Terms.AddRange(terms);
    Operator = @operator;
  }
}
