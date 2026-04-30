using Patches.Application.Contracts;
using Patches.CLI.App.Contracts;
using Patches.Shared.Queries;

namespace Patches.CLI.App;

public class ModulesList(IConsoleUIService ui, IHandler<ListModulesQuery, ListModulesQueryResult> listModulesHandler) : IScreen
{
    public async Task<string?> RunAsync()
    {
        var result = await listModulesHandler.HandleAsync(new ListModulesQuery());
        var moduleList = result.Modules;

        ui.Clear();
        ui.WriteLine("");
        ui.WriteLine("Modules:");
        ui.WriteLine("");

        if (moduleList.Count < 1)
        {
            ui.TextMiddle();
            ui.WriteLine("\tAdded modules will show up here");
        }

        foreach (var item in moduleList.Select((m, i) => new { module = m, index = i }))
        {
            ui.WriteLine($"{item.index + 1}\t{item.module.Name}");
        }

        ui.TextBottom();
        ui.WriteLine("Press any key to continue");
        ui.ReadKey(intercept: true);
        return null;
    }
}
