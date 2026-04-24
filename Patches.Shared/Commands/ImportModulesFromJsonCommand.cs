using System.ComponentModel.DataAnnotations;

namespace Patches.Shared.Commands;

public class ImportModulesFromJsonCommand : IValidatableObject
{
    public string Json { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Json))
            yield return new ValidationResult("JSON must be a non-empty string", [nameof(Json)]);
    }
}

public class ImportModulesFromJsonResult
{
    public int ImportedCount { get; set; }
    public int SkippedCount { get; set; }
}
