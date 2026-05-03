using Patches.Application.Contracts;
using Patches.CLI.App.Contracts;
using Patches.Shared.Queries;

namespace Patches.CLI.App;

public class ModulesList(
    IHandler<ListModulesQuery, ListModulesQueryResult> listModulesHandler,
    IConsoleUIService ui) : IScreen
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

        ui.WriteLine($"{"#",-4} {"Name",-20} {"HP",-5} {"U",-4} Description");
        ui.WriteLine(new string('-', 60));

        foreach (var item in moduleList.Select((m, i) => new { module = m, index = i }))
        {
            var hp = item.module.HorizontalPitch > 0 ? $"{item.module.HorizontalPitch}HP" : "";
            var u = item.module.VerticalUnits > 0 ? $"{item.module.VerticalUnits}U" : "";
            ui.WriteLine($"{item.index + 1,-4} {item.module.Name,-20} {hp,-5} {u,-4} {item.module.Description}");
        }

        ui.TextBottom();
        ui.WriteLine("Press any key to continue");
        ui.ReadKey(intercept: true);
        return null;
    }
}
