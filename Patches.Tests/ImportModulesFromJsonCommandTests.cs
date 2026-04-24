using System.ComponentModel.DataAnnotations;
using Patches.Shared.Commands;
using Xunit;

namespace Patches.Tests;

public class ImportModulesFromJsonCommandTests
{
    private static IList<ValidationResult> Validate(ImportModulesFromJsonCommand command)
    {
        var context = new ValidationContext(command);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(command, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Valid_Command_HasNoErrors()
    {
        var cmd = new ImportModulesFromJsonCommand { Json = "{\"response\":{\"success\":true}}" };
        Assert.Empty(Validate(cmd));
    }

    [Fact]
    public void EmptyJson_ReturnsValidationError()
    {
        var cmd = new ImportModulesFromJsonCommand { Json = "" };
        var results = Validate(cmd);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ImportModulesFromJsonCommand.Json)));
    }

    [Fact]
    public void WhitespaceJson_ReturnsValidationError()
    {
        var cmd = new ImportModulesFromJsonCommand { Json = "   " };
        var results = Validate(cmd);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ImportModulesFromJsonCommand.Json)));
    }
}
