using Spectre.Console;

namespace Patches.CLI;

public partial class PatchesCLI
{
    private async Task RenderLoadPatchScreenAsync()
    {
        UI.Clear();
        var result = await ListPatchesHandler.HandleAsync(new());
        var rows = new Rows(
            [.. result.Patches.Select((p, i) => new Markup($"{i+1}.\t[#FFD787]{p.Name}[/]"))]
        );

        AnsiConsole.MarkupLine("[#FFD787]Load a saved Patch[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Write(rows);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[#FFD787]Press any key to return to the home screen[/]");
        AnsiConsole.Cursor.SetPosition(0, Console.WindowHeight);
        UI.ReadKey();
    }
}
