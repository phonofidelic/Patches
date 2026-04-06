using System;
using Patches.Application.Contracts;
using Patches.Shared.Commands;

namespace Patches.CLI;

public partial class PatchesCLI
{
    private async Task AddModuleForm()
    {
        var command = new AddModuleCommand();
        UI.Clear();
        UI.WriteLine("\tAdd new Module:");
        UI.TextBottom();

        command.Name = UI.GetStringInput("Name: ", "Name must be a valid string");
        command.Description = UI.GetStringInput("Description: ", "Description must be a valid string", false);
        command.HorizontalPitch = UI.GetIntInput("HP (horizontal pitch): ", "HP must be a number");
        command.VerticalUnits = UI.GetIntInput("U (vertical units): ", "U must be a number");

        UI.Clear();
        UI.WriteLine("Adding new module...");
        UI.WriteLine("");
        var result = await AddModuleHandler.HandleAsync(command);
        
        UI.WriteLine($"Added new module '{result.Name}'");

        UI.CursorTop = Console.WindowHeight;

        UI.WriteLine("Any key to continue");
        UI.ReadKey();
        CurrentCommand = null;
    }
}
