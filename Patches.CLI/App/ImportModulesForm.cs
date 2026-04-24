using Patches.Shared.Commands;
using Spectre.Console;

namespace Patches.CLI;

public partial class PatchesCLI
{
    private async Task ImportModulesForm()
    {
        Console.Clear();
        Console.WriteLine("Import Modules from Modulargrid");
        Console.WriteLine(new string('-', 32));

        var url = AnsiConsole.Prompt(new TextPrompt<string>("Modulargrid endpoint URL: "));

        var result = await importModulesHandler.HandleAsync(new ImportModulesFromModulargridCommand
        {
            EndpointUrl = url
        });

        Console.WriteLine($"Imported: {result.ImportedCount} | Skipped: {result.SkippedCount}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }
}
