using Patches.Shared.Commands;
using Spectre.Console;

namespace Patches.CLI;

public partial class PatchesCLI
{
    private async Task ImportModulesFromJsonForm()
    {
        UI.ClearBuffer();
        UI.Clear();
        UI.WriteLine("Import Modules from Modulargrid JSON");
        UI.WriteLine(new string('-', 36));
        UI.WriteLine("Visit your rack URL with .json appended in a browser,");
        UI.WriteLine("copy the JSON, paste it below, then press Enter twice.");
        UI.WriteLine();

        var lines = new List<string>();
        string? line;
        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
        {
            lines.Add(line);
        }

        var json = string.Concat(lines);

        if (string.IsNullOrWhiteSpace(json))
        {
            UI.WriteLine("[red]No JSON provided. Import cancelled.[/]");
            UI.TextMiddle();
            UI.WriteLine("Press any key to continue");
            UI.TextBottom();
            UI.ReadKey();
            CurrentCommand = null;
            return;
        }

        UI.Clear();
        UI.WriteLine("Importing modules...", omitFromBuffer: true);

        ImportModulesFromJsonResult result;
        try
        {
            result = await ImportFromJsonHandler.HandleAsync(new ImportModulesFromJsonCommand { Json = json });
        }
        catch (Exception ex)
        {
            UI.Clear();
            UI.WriteLine($"[red]Import failed: {Markup.Escape(ex.Message)}[/]");
            UI.TextMiddle();
            UI.WriteLine("Press any key to continue");
            UI.TextBottom();
            UI.ReadKey();
            CurrentCommand = null;
            return;
        }

        UI.Clear();
        UI.WriteLine($"Imported: {result.ImportedCount} | Skipped: {result.SkippedCount}");
        UI.TextMiddle();
        UI.WriteLine("Press any key to continue");
        UI.TextBottom();
        UI.ReadKey();
        CurrentCommand = null;
    }
}
