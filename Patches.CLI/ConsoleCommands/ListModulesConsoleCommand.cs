using Patches.Application.Contracts;
using Patches.Shared.Queries;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Patches.CLI.ConsoleCommands;

public class ListModulesConsoleCommand(IHandler<ListModulesQuery, ListModulesQueryResult> handler)
    : AsyncCommand
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new ListModulesQuery());

        var table = new Table()
            .AddColumn("Name")
            .AddColumn("HP")
            .AddColumn("U")
            .AddColumn("Description");

        foreach (var m in result.Modules)
            table.AddRow(
                Markup.Escape(m.Name),
                Markup.Escape(m.HorizontalPitch.ToString()),
                Markup.Escape(m.VerticalUnits.ToString()),
                Markup.Escape(m.Description ?? ""));

        AnsiConsole.Write(table);
        return 0;
    }
}
