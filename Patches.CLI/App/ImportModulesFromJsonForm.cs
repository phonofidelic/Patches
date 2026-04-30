using Patches.Application.Contracts;
using Patches.CLI.App.Contracts;
using Patches.Shared.Commands;
using Spectre.Console;

namespace Patches.CLI.App;

public class ImportModulesFromJsonForm(
    IHandler<ImportModulesFromJsonCommand, ImportModulesFromJsonResult> importFromJsonHandler,
    IConsoleUIService ui) : IScreen
{
    public async Task<string?> RunAsync()
    {
        ui.ClearBuffer();
        ui.Clear();
        ui.WriteLine("Import Modules from Modulargrid JSON");
        ui.WriteLine(new string('-', 36));
        ui.WriteLine("Visit your rack URL with .json appended in a browser,");
        ui.WriteLine("copy the JSON, paste it below, then press Enter twice.");
        ui.WriteLine();

        var lines = new List<string>();
        string? line;
        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
        {
            lines.Add(line);
        }

        var json = string.Concat(lines);

        if (string.IsNullOrWhiteSpace(json))
        {
            ui.WriteLine("[red]No JSON provided. Import cancelled.[/]");
            ui.TextMiddle();
            ui.WriteLine("Press any key to continue");
            ui.TextBottom();
            ui.ReadKey();
            return null;
        }

        ui.Clear();
        ui.WriteLine("Importing modules...", omitFromBuffer: true);

        ImportModulesFromJsonResult result;
        try
        {
            result = await importFromJsonHandler.HandleAsync(new ImportModulesFromJsonCommand { Json = json });
        }
        catch (Exception ex)
        {
            ui.Clear();
            ui.WriteLine($"[red]Import failed: {Markup.Escape(ex.Message)}[/]");
            ui.TextMiddle();
            ui.WriteLine("Press any key to continue");
            ui.TextBottom();
            ui.ReadKey();
            return null;
        }

        ui.Clear();
        ui.WriteLine($"Imported: {result.ImportedCount} | Skipped: {result.SkippedCount}");
        ui.TextMiddle();
        ui.WriteLine("Press any key to continue");
        ui.TextBottom();
        ui.ReadKey();
        return null;
    }
}
