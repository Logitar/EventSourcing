namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class TypeExtensionsTests
{
  [Fact]
  public void Given_type_Then_correct_name()
  {
    Type type = typeof(TestAggregate);

    if (type.AssemblyQualifiedName != null)
    {
      Assert.Equal(type.AssemblyQualifiedName, type.GetName());
    }
    else if (type.FullName != null)
    {
      Assert.Equal(type.FullName, type.GetName());
    }
    else
    {
      Assert.Equal(type.Name, type.GetName());
    }
  }
}
