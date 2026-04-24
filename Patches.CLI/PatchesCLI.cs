using Patches.Application.Contracts;
using Patches.CLI.App;
using Patches.Shared.Commands;
using Patches.Shared.Queries;
using Spectre.Console;

namespace Patches.CLI;

public partial class PatchesCLI(
        IConsoleUIService ui,
        IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult> initializePatchMatrixHandler,
        IHandler<AddModuleCommand, AddModuleResult> addModuleHandler,
        IHandler<ListModulesQuery, ListModulesQueryResult> listModulesHandler,
        IHandler<ImportModulesFromModulargridCommand, ImportModulesFromModulargridResult> importModulesHandler)
{
    private readonly IConsoleUIService UI = ui;
    private readonly IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult> InitializePatchMatrixHandler = initializePatchMatrixHandler;
    private readonly IHandler<AddModuleCommand, AddModuleResult> AddModuleHandler = addModuleHandler;
    private readonly IHandler<ListModulesQuery, ListModulesQueryResult> ListModulesHandler = listModulesHandler;
    private readonly IHandler<ImportModulesFromModulargridCommand, ImportModulesFromModulargridResult> importModulesHandler = importModulesHandler;
    private InitializePatchMatrixResult? State { get; set; }
    private string? CurrentCommand { get; set; } = null;
    private IReadOnlyList<string> QuitCommands { get; } = ["q", "quit"];

    public async Task InitAsync()
    {
        State = await InitializePatchMatrixHandler.HandleAsync(new());
        await RunAsync();
    }

    private async Task RunAsync()
    {
        if (State is null) throw new Exception("State not initialized");

        var banner = new Banner("Patches", "v0.1");

        var helpTable = new HelpTable();

        var rootLayout = new Layout("Root")
            .SplitRows(
                new Layout("Top").Size(banner.Height + helpTable.Height + 6),
                new Layout("Bottom")
            );

        var top = rootLayout["Top"];
        var bottom = rootLayout["Bottom"];
        
        top.Update(new Rows(
            banner,
            helpTable
        ));

        bottom.Update(new Rows());

        while (!QuitCommands.Contains(CurrentCommand))
        {
            UI.Clear();
            AnsiConsole.Write(rootLayout);
            
            top.Update(new Rows(
                Console.WindowHeight > 38 ? banner : new Markup("[bold #FFD787]Patches[/]"),
                helpTable
            ));

            bottom.Update(new Rows());
 
            AnsiConsole.Cursor.SetPosition(0, Console.WindowHeight);
            CurrentCommand = AnsiConsole.Prompt(new TextPrompt<string?>("[#FFD787]>[/]").DefaultValue(null).ShowDefaultValue(false));

            switch (CurrentCommand)
            {
                case "import":
                case "im":
                    await ImportModulesForm();
                    break;

                case "add":
                case "a":
                    await AddModuleForm();
                    break;

                case "list":
                case "ls":
                case "l":
                    await ModulesList();
                    break;

                case "help":
                case "h":
                    HelpScreen();
                    break;

                case null:
                    break;

                default: 
                    bottom.Update(Align.Left(
                        new Rows(
                            new Markup($"[red]Unknown command: '{Markup.Escape(CurrentCommand)}'[/]")),
                        VerticalAlignment.Middle));
                    break;
            }
        }

        UI.WriteLine("Exiting");
        UI.Clear();
    }
}