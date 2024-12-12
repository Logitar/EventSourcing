using Logitar.EventSourcing.Demo.Application.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.EventSourcing.Demo.Models.Search;

public record SearchParameters
{
  public const string DescendingKeyword = "DESC";
  public const char SortSeparator = '.';

  [FromQuery(Name = "ids")]
  public IEnumerable<Guid>? Ids { get; set; }

  [FromQuery(Name = "search")]
  public IEnumerable<string>? SearchTerms { get; set; }

  [FromQuery(Name = "search_operator")]
  public SearchOperator? SearchOperator { get; set; }

  [FromQuery(Name = "sort")]
  public IEnumerable<string>? Sort { get; set; }

  [FromQuery(Name = "skip")]
  public int? Skip { get; set; }

  [FromQuery(Name = "limit")]
  public int? Limit { get; set; }

  public void Fill(SearchPayload payload)
  {
    if (Ids != null)
    {
      payload.Ids.AddRange(Ids);
    }

    if (SearchTerms != null)
    {
      payload.Search.Terms.AddRange(SearchTerms.Select(value => new SearchTerm(value)));
    }
    if (SearchOperator.HasValue)
    {
      payload.Search.Operator = SearchOperator.Value;
    }

    if (Sort != null)
    {
      foreach (string sort in Sort)
      {
        int index = sort.IndexOf(SortSeparator);
        if (index < 0)
        {
          payload.Sort.Add(new SortOption(sort));
        }
        else
        {
          bool isDescending = sort[..index].Equals(DescendingKeyword, StringComparison.InvariantCultureIgnoreCase);
          string field = sort[(index + 1)..];
          payload.Sort.Add(new SortOption(field, isDescending));
        }
      }
    }

    payload.Skip = Skip ?? 0;
    payload.Limit = Limit ?? 0;
  }
}
