using Patches.Shared.Commands;
using Spectre.Console;

namespace Patches.CLI;

// ToDo: Make this a separate 'ModuleForm' Component
public partial class PatchesCLI
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
                    if(input < 1)
                    {
                        UI.Clear();
                        UI.WritePreviousBuffer();
                        UI.VerticalPadding((Console.WindowHeight / 2) - 1);
                        return ValidationResult.Error("[red]Minimum horizontal pitch is 1 HP[/]");
                    }
                    
                    if(input > 104)
                    {
                        UI.Clear();
                        UI.WritePreviousBuffer();
                        UI.VerticalPadding((Console.WindowHeight / 2) - 1);
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

        UI.TextMiddle(omitFromBuffer: true);

        command.Name = AnsiConsole.Prompt(namePrompt);
   
        UI.Clear();
        UI.WritePreviousBuffer();
        UI.WriteLine($"Name: {command.Name}");
        UI.TextMiddle(omitFromBuffer: true);
        command.Description = AnsiConsole.Prompt(descriptionPrompt);
        
        UI.Clear();
        UI.WritePreviousBuffer();
        UI.WriteLine($"Description: {command.Description}");
        UI.TextMiddle(omitFromBuffer: true);
        command.HorizontalPitch = AnsiConsole.Prompt(horizontalPitchPrompt);
       
        UI.Clear();
        UI.WritePreviousBuffer();
        UI.WriteLine($"HP: {command.HorizontalPitch}");
        UI.TextMiddle(omitFromBuffer: true);
        command.VerticalUnits = AnsiConsole.Prompt(verticalUnitsPrompt);
    
        return command;
    }
    private async Task AddModuleForm()
    {
        UI.ClearBuffer();
        UI.Clear();
        UI.WriteLine("Add a new Module:");
        UI.WriteLine("\n");

        var command = await RenderForm();
        
        if (command == null) return;

        UI.Clear();
        UI.WriteLine();
        UI.WriteLine("Adding new module...", omitFromBuffer: true);
        UI.WriteLine();
        var result = await AddModuleHandler.HandleAsync(command);

        UI.Clear();
        UI.WriteLine($"Added new module '{result.Name}'");
        UI.WriteLine($"HP: \t{result.HorizontalPitch}");
        UI.WriteLine($"U: \t{result.VerticalUnits}");
        UI.TextMiddle();
        UI.WriteLine("Press any key to continue");
        UI.TextBottom();
        UI.ReadKey();
        CurrentCommand = null;
    }
}
