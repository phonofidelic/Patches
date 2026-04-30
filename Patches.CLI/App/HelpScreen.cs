using Patches.Application.Contracts;
using Patches.CLI.App.Contracts;
using Spectre.Console;

namespace Patches.CLI.App;

public class HelpScreen(IConsoleUIService ui, IAnsiConsole ansiConsole) : IScreen
{
    public Task<string?> RunAsync()
    {
        ui.Clear();
        ansiConsole.Write(Align.Center(new HelpTable(), VerticalAlignment.Middle));
        ansiConsole.Cursor.SetPosition(0, Console.WindowHeight);
        return Task.FromResult(ansiConsole.Prompt(
            new TextPrompt<string?>("[#FFD787]>[/]").DefaultValue(null).ShowDefaultValue(false)));
    }
}
