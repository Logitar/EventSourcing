using FluentValidation;
using FluentValidation.Validators;

namespace Logitar.EventSourcing.Demo.Domain.Validators;

internal class UrlValidator<T> : IPropertyValidator<T, string>
{
  private readonly HashSet<string> _schemes;

  public string Name { get; } = "UrlValidator";
  public IReadOnlySet<string> Schemes => _schemes;

  public UrlValidator(IEnumerable<string>? schemes = null)
  {
    if (schemes != null && schemes.Any())
    {
      _schemes = schemes.Select(scheme => scheme.ToLowerInvariant()).ToHashSet();
    }
    else
    {
      _schemes = ["http", "https"];
    }
  }

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return $"'{{PropertyName}}' must be a valid absolute URL using one of the following schemes: {string.Join(", ", _schemes)}";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    try
    {
      Uri uri = new(value, UriKind.Absolute);
      return _schemes.Contains(uri.Scheme.ToLowerInvariant());
    }
    catch (Exception)
    {
    }

    return false;
  }
}
