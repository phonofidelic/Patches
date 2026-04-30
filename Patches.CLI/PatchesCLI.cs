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

    public async Task InitAsync()
    {
        _state = await initHandler.HandleAsync(new());
        await RunAsync();
    }

    private async Task RunAsync()
    {
        if (_state is null) throw new Exception("State not initialized");

        var banner = new Banner("Patches", "v0.1");
        var helpTable = new HelpTable();

        var rootLayout = new Layout("Root")
            .SplitRows(
                new Layout("Top").Size(banner.Height + helpTable.Height + 6),
                new Layout("Bottom"));

        var top = rootLayout["Top"];
        var bottom = rootLayout["Bottom"];

        top.Update(new Rows(banner, helpTable));
        bottom.Update(new Rows());

        string? currentCommand = null;

        string? HandleUnknown(string? cmd)
        {
            bottom.Update(Align.Left(
                new Rows(new Markup($"[#FF5F5F]Unknown command: '{Markup.Escape(cmd ?? "")}'[/]")),
                VerticalAlignment.Middle));
            return null;
        }

        do
        {
            ui.Clear();
            ansiConsole.Write(rootLayout);

            top.Update(new Rows(
                Console.WindowHeight > 38 ? banner : new Markup("[bold #FFD787]Patches[/]"),
                helpTable));

            bottom.Update(new Rows());

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
