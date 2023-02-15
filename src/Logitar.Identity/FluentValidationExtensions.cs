using FluentValidation;
using System.Globalization;

namespace Logitar.Identity;

/// <summary>
/// Provides extension methods for FluentValidation.
/// </summary>
public static class FluentValidationExtensions
{
  /// <summary>
  /// Defines an 'alias' validator on the current rule builder. Validation will fail if the property
  /// is not composed on non-empty alphanumeric words separated by hyphens (-).
  /// </summary>
  /// <typeparam name="T">The type of the object being validated.</typeparam>
  /// <param name="ruleBuilder">The rule builder.</param>
  /// <returns>The rule builder.</returns>
  public static IRuleBuilder<T, string?> Alias<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(a => a == null || a.Split('-').All(w => !string.IsNullOrEmpty(w) && w.All(char.IsLetterOrDigit)))
      .WithErrorCode("AliasValidator")
      .WithMessage("'{PropertyName}' must be composed of non-empty alphanumeric words separated by hyphens (-).");
  }

  /// <summary>
  /// Defines an 'identifier' validator on the current rule builder. Validation will fail if the
  /// property is not a string composed only of letters, digits and underscores (_) or if it starts
  /// with a digit.
  /// </summary>
  /// <typeparam name="T">The type of the object being validated.</typeparam>
  /// <param name="ruleBuilder">The rule builder.</param>
  /// <returns>The rule builder.</returns>
  public static IRuleBuilder<T, string?> Identifier<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(i => i == null || (!string.IsNullOrEmpty(i) && !char.IsDigit(i.First()) && i.All(c => char.IsLetterOrDigit(c) || c == '_')))
      .WithErrorCode("IdentifierValidator")
      .WithMessage("'{PropertyName}' can only include letters, digits and underscores (_) and must not start with a digit.");
  }

  /// <summary>
  /// Defines a 'locale' validator on the current rule builder. Validation will fail if the property
  /// is not an instance of the <see cref="CultureInfo"/> class with a non-empty name and a LCID
  /// different from 4096.
  /// </summary>
  /// <typeparam name="T">The type of the object being validated.</typeparam>
  /// <param name="ruleBuilder">The rule builder.</param>
  /// <returns>The rule builder.</returns>
  public static IRuleBuilder<T, CultureInfo?> Locale<T>(this IRuleBuilder<T, CultureInfo?> ruleBuilder)
  {
    return ruleBuilder.Must(c => c == null || (!string.IsNullOrEmpty(c.Name) && c.LCID != 4096))
      .WithErrorCode("LocaleValidator")
      .WithMessage("'{PropertyName}' must have a non-empty name and its LCID must be different from 4096.");
  }

  /// <summary>
  /// Defines an 'null or not empty' validator on the current rule builder. Validation will fail if
  /// the property is an empty or white space only string.
  /// </summary>
  /// <typeparam name="T">The type of the object being validated.</typeparam>
  /// <param name="ruleBuilder">The rule builder.</param>
  /// <returns>The rule builder.</returns>
  public static IRuleBuilder<T, string?> NullOrNotEmpty<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(s => s == null || !string.IsNullOrWhiteSpace(s))
      .WithErrorCode("NullOrNotEmptyValidator")
      .WithMessage("'{PropertyName}' must be null or not empty or white spaces only.");
  }

  /// <summary>
  /// Defines an 'url' validator on the current rule builder. Validation will fail if the property is
  /// not a well formed URL.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="ruleBuilder"></param>
  /// <returns></returns>
  public static IRuleBuilder<T, string?> Url<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(u => Uri.IsWellFormedUriString(u, UriKind.RelativeOrAbsolute))
      .WithErrorCode("UrlValidator")
      .WithMessage("'{PropertyName}' must be a well formed URL.");
  }
}
