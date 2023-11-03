namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class TypeExtensionsTests
{
  private const string AssemblyName = "Logitar.EventSourcing.UnitTests";
  private const string TypeName = "Logitar.EventSourcing.TypeMock";

  private readonly Assembly _assembly = Assembly.Load(AssemblyName);

  [Fact(DisplayName = "GetName: it returns the correct type name.")]
  public void GetName_it_returns_the_correct_type_name()
  {
    TypeMock type = new(_assembly, TypeName);
    Assert.Equal($"{TypeName}, {AssemblyName}", type.GetName());
  }

  [Fact(DisplayName = "GetName: it throws ArgumentException when assembly name is null.")]
  public void GetName_it_throws_ArgumentException_when_assembly_name_is_null()
  {
    AssemblyMock assembly = new(name: null);
    TypeMock type = new(assembly, TypeName);
    var exception = Assert.Throws<ArgumentException>(type.GetName);
    Assert.StartsWith("The assembly simple name is required.", exception.Message);
    Assert.Equal("type", exception.ParamName);
  }

  [Fact(DisplayName = "GetName: it throws ArgumentException when FullName is null.")]
  public void GetName_it_throws_ArgumentException_when_FullName_is_null()
  {
    TypeMock type = new(_assembly, fullName: null);
    var exception = Assert.Throws<ArgumentException>(type.GetName);
    Assert.StartsWith("The FullName is required.", exception.Message);
    Assert.Equal("type", exception.ParamName);
  }
}
