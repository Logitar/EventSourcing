namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class StringExtensionsTests
{
  [Theory]
  [InlineData("Qe0YB5A/MUaRmUk+CXGRsg==")]
  [InlineData("Qe0YB5AcMUaRmUkpCXGRsgOy")]
  public void Given_base64_string_Then_uri_safe_base64(string s)
  {
    string expected = s.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    string actual = s.ToUriSafeBase64();

    Assert.Equal(expected, actual);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("    ")]
  public void Given_null_or_empty_or_white_spaces_When_clean_trimmed_Then_null(string s)
  {
    Assert.Null(s.CleanTrim());
  }

  [Theory]
  [InlineData("Hello World!", "Hello")]
  public void Given_string_contains_pattern_When_removed_Then_pattern_is_removed(string s, string pattern)
  {
    Assert.Equal(s.Replace(pattern, string.Empty), s.Remove(pattern));
  }

  [Theory]
  [InlineData("Hello World!", "Test")]
  public void Given_string_does_not_contain_pattern_When_removed_Then_same_string(string s, string pattern)
  {
    Assert.Equal(s, s.Remove(pattern));
  }

  [Theory]
  [InlineData("Test")]
  [InlineData(" Hello  World!   ")]
  public void Given_string_When_clean_trimmed_Then_trimmed(string s)
  {
    Assert.Equal(s.Trim(), s.CleanTrim());
  }

  [Theory]
  [InlineData("knqQH2V/ukW0IK3+UYlFaw==")]
  [InlineData("knqQH2VZukW0IK30UYlFaw8x")]
  public void Given_uri_safe_base64_string_Then_correctly_parsed(string base64)
  {
    byte[] bytes = Convert.FromBase64String(base64);
    string s = Convert.ToBase64String(bytes).ToUriSafeBase64();
    byte[] other = Convert.FromBase64String(s.FromUriSafeBase64());
    Assert.Equal(bytes, other);
  }
}
