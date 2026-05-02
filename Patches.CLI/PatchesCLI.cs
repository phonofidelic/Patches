using Patches.Application.Contracts;
using Patches.CLI.App;
using Patches.Shared.Commands;
using Spectre.Console;

namespace Patches.CLI;

public class PatchesCLI(
    IConsoleUIService ui,
    IAnsiConsole ansiConsole,
    IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult> initHandler,
    HelpScreen helpScreen,
    ModulesList modulesListScreen,
    AddModuleForm addModuleFormScreen,
    ImportModulesFromJsonForm importFromJsonScreen,
    PatchMatrixScreen patchMatrixScreen,
    LoadPatchScreen loadPatchScreen)
{
    private InitializePatchMatrixResult? _state;
    private static readonly IReadOnlyList<string> QuitCommands = ["q", "quit"];
    private static string DbPath { get; set; } = "";
    public async Task InitAsync(string dbPath)
    {
        _state = await initHandler.HandleAsync(new());
        DbPath = dbPath;
        await RunAsync();
    }

    private async Task RunAsync()
    {
        if (_state is null) throw new Exception("State not initialized");

        var banner = new Banner("Patches", "v0.1");

        var dbInfo = new Panel(new TextPath(DbPath)
                .RootColor(Color.Gray)
                .SeparatorColor(Color.Gray)
                .StemColor(Color.Gray)
                .LeafStyle(new Style().Decoration(Decoration.Bold)))
            .Header("Db path:")
            .RoundedBorder();

        var helpTable = new HelpTable();

        var rootLayout = new Layout("Root");

        rootLayout.Update(new Rows(
            banner,
            dbInfo,
            helpTable));

        string? currentCommand = null;

        string? HandleUnknown(string? cmd)
        {
            rootLayout.Update(new Rows(
                banner,
                dbInfo,
                helpTable,
                new Markup($"[#FF5F5F]Unknown command: '{Markup.Escape(cmd ?? "")}'[/]")));
            return null;
        }

        do
        {
            ui.Clear();
            ansiConsole.Write(rootLayout);

            rootLayout.Update(new Rows(
                banner,
                dbInfo,
                helpTable));

            ansiConsole.Cursor.SetPosition(0, Console.WindowHeight);
            currentCommand ??= ansiConsole.Prompt(new TextPrompt<string?>("[#FFD787]>[/]").DefaultValue(null).ShowDefaultValue(false));

            currentCommand = currentCommand switch
            {
                "import-json" or "ij"            => await importFromJsonScreen.RunAsync(),
                "add"        or "a"               => await addModuleFormScreen.RunAsync(),
                "list"       or "ls" or "l"       => await modulesListScreen.RunAsync(),
                "new-patch"  or "np" or "new"     => await patchMatrixScreen.RunAsync(),
                "load-patch" or "lp"              => await loadPatchScreen.RunAsync(),
                "help"       or "h"               => await helpScreen.RunAsync(),
                "quit"       or "q" or null       => currentCommand,
                _                                 => HandleUnknown(currentCommand),
            };

        } while (!QuitCommands.Contains(currentCommand));

        ui.WriteLine("Exiting");
        ui.Clear();
    }
}
