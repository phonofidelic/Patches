using Patches.Application.Contracts;
using Patches.Shared.Commands;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Patches.CLI.ConsoleCommands;

public class ImportModulesConsoleCommand(IHandler<ImportModulesFromJsonCommand, ImportModulesFromJsonResult> handler)
    : AsyncCommand<ImportModulesConsoleCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("--file|-f")]
        [Description("Path to the JSON file")]
        public required string FilePath { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync(settings.FilePath);
        var cmd = new ImportModulesFromJsonCommand { Json = json };
        var result = await handler.HandleAsync(cmd);
        AnsiConsole.MarkupLine($"[green]Imported {result.ImportedCount} module(s).[/]");
        return 0;
    }
}
