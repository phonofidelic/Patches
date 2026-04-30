using Patches.Application.Contracts;
using Patches.CLI.App.Contracts;
using Patches.Shared.Dtos;
using Patches.Shared.Queries;
using Spectre.Console;

namespace Patches.CLI.App;

public class LoadPatchScreen(
    IConsoleUIService ui,
    IAnsiConsole ansiConsole,
    IHandler<ListPatchesQuery, ListPatchesQueryResult> listPatchesHandler,
    PatchMatrixScreen patchMatrixScreen) : IScreen
{
    public async Task<string?> RunAsync()
    {
        var result = await listPatchesHandler.HandleAsync(new ListPatchesQuery());
        if (result.Patches.Count < 1)
            return null;

        var choices = new SelectionPrompt<PatchListItemDto>()
            .Title("[#FFD787 bold]Load a saved Patch:[/]")
            .HighlightStyle(new Style(Color.LightGoldenrod2_2, Console.BackgroundColor, Decoration.Bold))
            .UseConverter(p => $"[#FFF]{p.Name}[/]")
            .AddChoices(result.Patches);

        ui.Clear();
        var loadPatchResult = new LoadPatchResult(ansiConsole.Prompt(choices));

        if (loadPatchResult.SelectedPatch != null)
            await patchMatrixScreen.RunAsync(loadPatchResult.SelectedPatch);

        return null;
    }
}
