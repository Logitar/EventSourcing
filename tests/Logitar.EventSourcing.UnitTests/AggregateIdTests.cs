namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class AggregateIdTests
{
  [Fact]
  public void Given_default_value_Then_correct_hash_code()
  {
    AggregateId id = new();
    Assert.Equal(string.Empty.GetHashCode(), id.GetHashCode());
  }

  [Theory]
  [InlineData("SYSTEM")]
  public void Given_different_type_Then_not_equal(string value)
  {
    AggregateId id = new(value);
    Assert.False(id.Equals(value));
  }

  [Theory]
  [InlineData("D959A3DB-AEB6-4BA6-BD1F-4C5D58173F4E", "AC564417-896D-41B8-8ACC-7E53DEDAC491")]
  public void Given_different_values_Then_not_equal(string value1, string value2)
  {
    AggregateId id = new(value1);
    AggregateId other = new(value2);
    Assert.True(id != other);
  }

  [Theory]
  [InlineData("02A91EBB-5087-4EE3-8374-24B1389BC4E7")]
  public void Given_Guid_in_constructor_Then_with_uri_safe_base64_value(string value)
  {
    Guid guid = Guid.Parse(value);
    string expected = Convert.ToBase64String(guid.ToByteArray()).ToUriSafeBase64();

    AggregateId id = new(guid);
    Assert.Equal(expected, id.Value);
  }

  [Fact]
  public void Given_new_ID_Then_random_Guid_generated()
  {
    AggregateId id = AggregateId.NewId();
    _ = id.ToGuid();
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("    ")]
  public void Given_null_or_white_space_string_value_Then_ArgumentException_is_thrown(string? value)
  {
    Assert.Throws<ArgumentException>(() => new AggregateId(value!));
  }

  [Theory]
  [InlineData("SYSTEM")]
  public void Given_same_type_and_value_Then_equal(string value)
  {
    AggregateId id = new(value);
    AggregateId other = new(value);
    Assert.True(id.Equals(other));
    Assert.True(id == other);
  }

  [Theory]
  [InlineData("SYSTEM")]
  public void Given_String_in_constructor_Then_trimmed_value(string value)
  {
    value = $" {value}   ";

    AggregateId id = new(value);
    Assert.Equal(value.Trim(), id.Value);
  }

  [Theory]
  [InlineData("SYSTEM")]
  public void Given_value_Then_correct_hash_code(string value)
  {
    AggregateId id = new(value);
    Assert.Equal(value.GetHashCode(), id.GetHashCode());
  }

  [Theory]
  [InlineData("SYSTEM")]
  public void Given_value_Then_correct_string(string value)
  {
    AggregateId id = new(value);
    Assert.Equal(value, id.ToString());
  }
}
