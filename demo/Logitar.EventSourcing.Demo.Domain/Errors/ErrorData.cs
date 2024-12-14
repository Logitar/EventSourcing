namespace Logitar.EventSourcing.Demo.Domain.Errors;

public record ErrorData
{
  public string Key { get; set; } = string.Empty;
  public object? Value { get; set; }

  public ErrorData()
  {
  }

  public ErrorData(KeyValuePair<string, object?> data)
  {
    Key = data.Key;
    Value = data.Value;
  }

  public ErrorData(string key, object? value)
  {
    Key = key;
    Value = value;
  }
}
