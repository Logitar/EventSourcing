using Logitar.EventSourcing.Demo.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.EventSourcing.Demo.Extensions;

internal static class ProblemDetailsExtensions
{
  public static void Populate(this ProblemDetails problemDetails, Exception exception)
  {
    if (exception is ErrorException errorException)
    {
      problemDetails.Populate(errorException.Error);
    }
    else
    {
      problemDetails.Extensions.TryAdd("code", exception.GetErrorCode());
    }
  }

  public static void Populate(this ProblemDetails problemDetails, Error error)
  {
    problemDetails.Title = FormatToTitle(error.Code);
    problemDetails.Detail = error.Message;
    problemDetails.Extensions.TryAdd("code", error.Code);
  }

  private static string FormatToTitle(string code)
  {
    List<string> words = new(capacity: code.Length);

    StringBuilder word = new();
    for (int i = 0; i < code.Length; i++)
    {
      char? previous = (i > 0) ? code[i - 1] : null;
      char current = code[i];
      char? next = (i < code.Length - 1) ? code[i + 1] : null;

      if (char.IsUpper(current) && ((previous.HasValue && char.IsLower(previous.Value)) || (next.HasValue && char.IsLower(next.Value))))
      {
        if (word.Length > 0)
        {
          words.Add(word.ToString());
          word.Clear();
        }
      }

      word.Append(current);
    }
    if (word.Length > 0)
    {
      words.Add(word.ToString());
    }

    return string.Join(' ', words);
  }
}
