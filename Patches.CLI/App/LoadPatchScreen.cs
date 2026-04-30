using Patches.Shared.Dtos;
using Patches.Shared.Queries;
using Spectre.Console;

namespace Patches.CLI;


public partial class PatchesCLI
{
    private async Task RenderLoadPatchScreenAsync()
    {
        PatchListItemDto? selectedPatch;
        var result = await ListPatchesHandler.HandleAsync(new ListPatchesQuery());

        if (result.Patches.Count < 1)
        {
            // ToDo: Show prompt to create a new patch or return to home screen
            CurrentCommand = null;
            return;
        }

        var choices = new SelectionPrompt<PatchListItemDto>()
        {
            Converter = p => $"[#FFF]{p.Name}[/]",
        };
        choices
            .Title("[#FFD787 bold]Load a saved Patch:[/]")
            .HighlightStyle(new Style(Color.LightGoldenrod2_2, Console.BackgroundColor, Decoration.Bold))
            .AddChoices(result.Patches);
        
        UI.Clear();
        selectedPatch = AnsiConsole.Prompt(choices);

        if (selectedPatch != null)
            await RenderPatchMatrixScreenAsync(selectedPatch);

        CurrentCommand = null;
    }
}
