using Patches.Application.Contracts;
using Patches.Shared.Commands;
using Patches.Shared.Queries;

namespace Patches.CLI;

public partial class PatchesCLI(
        IConsoleUIService ui,
        IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult> initializePatchMatrixHandler,
        IHandler<AddModuleCommand, AddModuleResult> addModuleHandler,
        IHandler<ListModulesQuery, ListModulesQueryResult> listModulesHandler)
{
    private readonly IConsoleUIService UI = ui;
    private readonly IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult> InitializePatchMatrixHandler = initializePatchMatrixHandler;
    private readonly IHandler<AddModuleCommand, AddModuleResult> AddModuleHandler = addModuleHandler;
    private readonly IHandler<ListModulesQuery, ListModulesQueryResult> ListModulesHandler = listModulesHandler;    private InitializePatchMatrixResult? State { get; set; }
    private string? CurrentCommand { get; set; } = null;

    public async Task InitAsync()
    {
        State = await InitializePatchMatrixHandler.HandleAsync(new());
        await RunAsync();
    }

    private async Task RunAsync()
    {
        if (State is null) throw new Exception("State not initialized");

        while (CurrentCommand != "q")
        {
            UI.Clear();
            UI.DisplayHelp();
            UI.TextBottom();
            if (!string.IsNullOrWhiteSpace(CurrentCommand))
                        UI.WriteLine($"Unknown command: '{CurrentCommand}'");
            CurrentCommand = Console.ReadLine() ?? "";

            switch (CurrentCommand)
            {
                case "add":
                    await AddModuleForm();
                    break;
                case "list":
                    await ModulesList();
                    break;

                default: 
                    break;

            }
        }

        Console.WriteLine("Exiting");
    }
}