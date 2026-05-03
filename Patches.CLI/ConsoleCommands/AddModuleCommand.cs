using Patches.Application.Contracts;
using Patches.Shared.Commands;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Patches.CLI.ConsoleCommands;

public class AddModuleCommand(IHandler<Patches.Shared.Commands.AddModuleCommand, AddModuleResult> handler)
    : AsyncCommand<AddModuleCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("--name|-n")]
        [Description("Module name")]
        public required string Name { get; init; }

        [CommandOption("--hp")]
        [Description("Horizontal pitch (1-104)")]
        public required int HorizontalPitch { get; init; }

        [CommandOption("--vertical-units|-u")]
        [DefaultValue(3)]
        [Description("Vertical units (default 3)")]
        public int VerticalUnits { get; init; } = 3;

        [CommandOption("--description|-d")]
        [Description("Optional description")]
        public string? Description { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var cmd = new Patches.Shared.Commands.AddModuleCommand
        {
            Name = settings.Name,
            HorizontalPitch = settings.HorizontalPitch,
            VerticalUnits = settings.VerticalUnits,
            Description = settings.Description ?? string.Empty,
        };

        var result = await handler.HandleAsync(cmd);
        AnsiConsole.MarkupLine($"[green]Added:[/] {Markup.Escape(result.Name)}  HP={result.HorizontalPitch}  U={result.VerticalUnits}");
        return 0;
    }
}
