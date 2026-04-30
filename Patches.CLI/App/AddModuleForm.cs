using Patches.Application.Contracts;
using Patches.CLI.App.Contracts;
using Patches.Shared.Commands;
using Spectre.Console;

namespace Patches.CLI.App;

public class AddModuleForm(IConsoleUIService ui, IAnsiConsole ansiConsole, IHandler<AddModuleCommand, AddModuleResult> addModuleHandler) : IScreen
{
    private async Task<AddModuleCommand?> RenderForm()
    {
        var command = new AddModuleCommand();
        var namePrompt = new TextPrompt<string>("Name: ")
            .Validate(
                input => !string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input),
                "Name must be a valid string");
        var descriptionPrompt = new TextPrompt<string>("Description: ")
        {
            AllowEmpty = true
        };
        var horizontalPitchPrompt = new TextPrompt<int>("HP: ")
            .Validate(
                input =>
                {
                    if (input < 1)
                    {
                        ui.Clear();
                        ui.WritePreviousBuffer();
                        ui.VerticalPadding((Console.WindowHeight / 2) - 1);
                        return ValidationResult.Error("[red]Minimum horizontal pitch is 1 HP[/]");
                    }

                    if (input > 104)
                    {
                        ui.Clear();
                        ui.WritePreviousBuffer();
                        ui.VerticalPadding((Console.WindowHeight / 2) - 1);
                        return ValidationResult.Error("[red]Maximum horizontal pitch is 104 HP[/]");
                    }

                    return ValidationResult.Success();
                });
        var verticalUnitsPrompt = new TextPrompt<int>("U: ")
            .DefaultValue(3)
            .ShowDefaultValue()
            .Validate(
                input => input > 0,
                "Minimum vertical units is 1 U"
            );

        ui.TextMiddle(omitFromBuffer: true);

        command.Name = ansiConsole.Prompt(namePrompt);

        ui.Clear();
        ui.WritePreviousBuffer();
        ui.WriteLine($"Name: {command.Name}");
        ui.TextMiddle(omitFromBuffer: true);
        command.Description = ansiConsole.Prompt(descriptionPrompt);

        ui.Clear();
        ui.WritePreviousBuffer();
        ui.WriteLine($"Description: {command.Description}");
        ui.TextMiddle(omitFromBuffer: true);
        command.HorizontalPitch = ansiConsole.Prompt(horizontalPitchPrompt);

        ui.Clear();
        ui.WritePreviousBuffer();
        ui.WriteLine($"HP: {command.HorizontalPitch}");
        ui.TextMiddle(omitFromBuffer: true);
        command.VerticalUnits = ansiConsole.Prompt(verticalUnitsPrompt);

        return command;
    }

    public async Task<string?> RunAsync()
    {
        ui.ClearBuffer();
        ui.Clear();
        ui.WriteLine("Add a new Module:");
        ui.WriteLine("\n");

        var command = await RenderForm();

        if (command == null) return null;

        ui.Clear();
        ui.WriteLine();
        ui.WriteLine("Adding new module...", omitFromBuffer: true);
        ui.WriteLine();
        var result = await addModuleHandler.HandleAsync(command);

        ui.Clear();
        ui.WriteLine($"Added new module '{result.Name}'");
        ui.WriteLine($"HP: \t{result.HorizontalPitch}");
        ui.WriteLine($"U: \t{result.VerticalUnits}");
        ui.TextMiddle();
        ui.WriteLine("Press any key to continue");
        ui.TextBottom();
        ui.ReadKey();
        return null;
    }
}
