using System;
using Patches.Application.Commands;
using Patches.Application.Contracts;

namespace Patches.CLI;

public class PatchesCLI(
        IConsoleUIService ui,
        IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult> initializePatchMatrixHandler,
        IHandler<AddModuleCommand, AddModuleResult> addModuleHandler)
{
    private readonly IConsoleUIService UI = ui;
    private readonly IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult> InitializePatchMatrixHandler = initializePatchMatrixHandler;
    private readonly IHandler<AddModuleCommand, AddModuleResult> AddModuleHandler = addModuleHandler;
    private InitializePatchMatrixResult? State { get; set; }

    public async Task InitAsync()
    {
        State = await InitializePatchMatrixHandler.HandleAsync(new());
        await RunAsync();
    }

    private async Task RunAsync()
    {
        if (State is null) throw new Exception("State not initialized");
        UI.Clear();
        UI.DisplayHelp();
        Console.CursorTop = Console.WindowHeight;
        string input = Console.ReadLine() ?? "";

        while (input != "q")
        {
            switch (input)
            {
                case "add":
                    var command = new AddModuleCommand();
                    Console.Clear();
                    Console.WriteLine("Add new Module:");
                    Console.CursorTop = Console.WindowHeight;

                    command.Name = UI.GetStringInput("Name: ", "Name must be a valid string");
                    command.Description = UI.GetStringInput("Description: ", "Description must be a valid string", false);
                    command.HorizontalPitch = UI.GetIntInput("HP (horizontal pitch): ", "HP must be a number");
                    command.VerticalUnits = UI.GetIntInput("U (vertical units): ", "U must be a number");

                    UI.Clear();
                    UI.WriteLine("Adding new module...");
                    UI.WriteLine("");
                    var result = await AddModuleHandler.HandleAsync(command);
                    State.Modules.Add(new ModuleDisplay(id: result.Id, name: result.Name));
                    
                    UI.WriteLine(State.ToString());

                    Console.CursorTop = Console.WindowHeight;

                    UI.WriteLine("Any key to continue");
                    Console.ReadKey();
                    break;
                default: 
                    Console.WriteLine();
                    break;

            }
            UI.Clear();
            UI.DisplayHelp();
            Console.CursorTop = Console.WindowHeight;
            input = Console.ReadLine() ?? "";
            Console.WriteLine("Continue");
        }

        Console.WriteLine("Exiting");
    }
}