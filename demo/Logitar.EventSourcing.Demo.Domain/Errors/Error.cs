namespace Logitar.EventSourcing.Demo.Domain.Errors;

public record Error
{
  public string Code { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
  public List<ErrorData> Data { get; set; } = [];

  public Error()
  {
  }

  public Error(string code, string message, IEnumerable<ErrorData>? data = null)
  {
    Code = code;
    Message = message;

    if (data != null)
    {
      Data.AddRange(data);
    }
  }

  public void AddData(KeyValuePair<string, object?> data)
  {
    Data.Add(new ErrorData(data));
  }
  public void AddData(string key, object? value)
  {
    Data.Add(new ErrorData(key, value));
  }
}
