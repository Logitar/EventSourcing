namespace Logitar.Identity;

/// <summary>
/// Provides extensions for <see cref="Dictionary{TKey, TValue}"/> classes.
/// </summary>
public static class DictionaryExtensions
{
  /// <summary>
  /// Adds the elements of the specified collection in the dictionary.
  /// </summary>
  /// <typeparam name="TKey">The type of the dictionary keys.</typeparam>
  /// <typeparam name="TValue">The type of the dictionary values.</typeparam>
  /// <param name="dictionary">The dictionary to add elements to.</param>
  /// <param name="pairs">The element pairs to add to the dictionary.</param>
  public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
  {
    foreach (KeyValuePair<TKey, TValue> pair in pairs)
    {
      dictionary.Add(pair);
    }
  }
}
