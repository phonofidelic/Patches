using System.ComponentModel.DataAnnotations;
using Patches.Shared.Commands;

namespace Patches.Tests;

public class AddModuleCommandTests
{
    private static IList<ValidationResult> Validate(AddModuleCommand command)
    {
        var context = new ValidationContext(command);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(command, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Valid_Command_HasNoErrors()
    {
        var cmd = new AddModuleCommand { Name = "Maths", HorizontalPitch = 1, VerticalUnits = 3 };
        Assert.Empty(Validate(cmd));
    }

    [Fact]
    public void EmptyName_ReturnsValidationError()
    {
        var cmd = new AddModuleCommand { Name = "", HorizontalPitch = 4, VerticalUnits = 3 };
        var results = Validate(cmd);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AddModuleCommand.Name)));
    }

    [Fact]
    public void WhitespaceName_ReturnsValidationError()
    {
        var cmd = new AddModuleCommand { Name = "   ", HorizontalPitch = 4, VerticalUnits = 3 };
        var results = Validate(cmd);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AddModuleCommand.Name)));
    }

    [Fact]
    public void HorizontalPitch_Zero_ReturnsValidationError()
    {
        var cmd = new AddModuleCommand { Name = "Maths", HorizontalPitch = 0, VerticalUnits = 3 };
        var results = Validate(cmd);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AddModuleCommand.HorizontalPitch)));
    }

    [Fact]
    public void HorizontalPitch_105_ReturnsValidationError()
    {
        var cmd = new AddModuleCommand { Name = "Maths", HorizontalPitch = 105, VerticalUnits = 3 };
        var results = Validate(cmd);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AddModuleCommand.HorizontalPitch)));
    }

    [Fact]
    public void HorizontalPitch_104_IsValid()
    {
        var cmd = new AddModuleCommand { Name = "Maths", HorizontalPitch = 104, VerticalUnits = 3 };
        Assert.Empty(Validate(cmd));
    }

    [Fact]
    public void VerticalUnits_Zero_ReturnsValidationError()
    {
        var cmd = new AddModuleCommand { Name = "Maths", HorizontalPitch = 4, VerticalUnits = 0 };
        var results = Validate(cmd);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AddModuleCommand.VerticalUnits)));
    }

    [Fact]
    public void VerticalUnits_Six_ReturnsValidationError()
    {
        var cmd = new AddModuleCommand { Name = "Maths", HorizontalPitch = 4, VerticalUnits = 6 };
        var results = Validate(cmd);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AddModuleCommand.VerticalUnits)));
    }

    [Fact]
    public void VerticalUnits_Five_IsValid()
    {
        var cmd = new AddModuleCommand { Name = "Maths", HorizontalPitch = 4, VerticalUnits = 5 };
        Assert.Empty(Validate(cmd));
    }
}
