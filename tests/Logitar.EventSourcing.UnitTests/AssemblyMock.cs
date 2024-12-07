namespace Logitar.EventSourcing;

internal class AssemblyMock : Assembly
{
  public AssemblyMock(string? name)
  {
    Name = name;
  }

  public string? Name { get; }

  public override AssemblyName GetName() => Name == null ? new AssemblyName() : new AssemblyName(Name);
}
