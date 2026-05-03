using Patches.Application.Contracts;
using Patches.Shared.Queries;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Patches.CLI.ConsoleCommands;

public class ListModulesCommand(IHandler<ListModulesQuery, ListModulesQueryResult> handler)
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
            table.AddRow(m.Name, m.HorizontalPitch.ToString(), m.VerticalUnits.ToString(), m.Description ?? "");

        AnsiConsole.Write(table);
        return 0;
    }
}
