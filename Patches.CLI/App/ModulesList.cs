using System;
using AutoMapper;
using Patches.Application.Handlers;
using Patches.Shared.Queries;

namespace Patches.CLI;

public partial class PatchesCLI
{
    private async Task ModulesList()
    {
        var result = await ListModulesHandler.HandleAsync(new ListModulesQuery());
        var moduleList = result.Modules;

        UI.Clear();
        UI.WriteLine("");
        UI.WriteLine("Modules:");
        UI.WriteLine("");

        if (moduleList.Count < 1)
        {
            UI.TextMiddle();
            UI.WriteLine("\tAdded modules will show up here");
        }

        foreach (var item in moduleList.Select((m, i) => new {module = m, index = i }))
        {
            UI.WriteLine($"{item.index + 1}\t{item.module.Name}");
        }

        UI.TextBottom();
        UI.WriteLine("Press any key to continue");
        UI.ReadKey(intercept: true);
        CurrentCommand = null;
    }
}
