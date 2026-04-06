namespace Patches.Application.Commands;

public class InitializePatchMatrixCommand
{

}

public class InitializePatchMatrixResult
{
    public ICollection<ModuleDisplay> Modules { get; set; } = [];
    public ICollection<PatchDisplay> Patches { get; set; } = [];

    public override string ToString()
    {
        string modules = "";
        foreach (var module in Modules)
        {
            modules += module.Name + "\n";
        }
        return @$"Modules:
        {modules}";
    }
}

public class ModuleDisplay(
    Guid id,
    string name)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
}

public class PatchDisplay(
    int id,
    string name)
{
   public int Id { get; set; } = id; 
   public string Name { get; set; } = name;
}