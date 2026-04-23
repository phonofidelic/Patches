using Patches.CLI.App;
using Spectre.Console;

namespace Patches.CLI;

public partial class PatchesCLI
{
    public void HelpScreen()
    {
        UI.Clear();
        AnsiConsole.Write(Align.Center(new HelpTable(), VerticalAlignment.Middle));
        

        AnsiConsole.Cursor.SetPosition(0, Console.WindowHeight);
            CurrentCommand = AnsiConsole.Prompt(new TextPrompt<string?>("[#FFD787]>[/]").DefaultValue(null).ShowDefaultValue(false));
    }
}
