namespace Logitar.EventSourcing.Demo.Application.Search;

public record SearchTerm
{
  public string Value { get; set; } = string.Empty;

  public SearchTerm()
  {
  }

  public SearchTerm(string value)
  {
    Value = value;
  }
}
