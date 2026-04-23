using System.ComponentModel.DataAnnotations;

namespace Patches.Shared.Commands;

public class AddModuleCommand : IValidatableObject
{
    public string Name { get; set; } = string.Empty;

    [Range(1, 104)]
    public int HorizontalPitch { get; set; }

    [Range(1,5)]
    public int VerticalUnits { get; set; } = 3;
    public string Description { get; set; } = string.Empty;
    public string? Vendor { get; set; }

    public AddModuleCommand()
    {
        
    }
    public AddModuleCommand(
        string name,
        int hp,
        string? description,
        string? vendor,
        int u = 3
    )
    {
        Name = name;
        HorizontalPitch = hp;
        VerticalUnits = u;
        Description = description ?? string.Empty;
        Vendor = vendor;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("Module name must be a non-empty string", [nameof(Name)]);

        if (HorizontalPitch < 1)
            yield return new ValidationResult("Minimum horizontal pitch is 1 HP", [nameof(HorizontalPitch)]);

        if (VerticalUnits < 1)
            yield return new ValidationResult("Minimum vertical units is 1 U", [nameof(VerticalUnits)]);
    }
}

public class AddModuleResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int HorizontalPitch { get; set; }
    public int VerticalUnits { get; set; }
    public IReadOnlyList<ModuleConnectionPoint> ConnectionPoints { get; set; } = [];
}

public class ModuleConnectionPoint
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}