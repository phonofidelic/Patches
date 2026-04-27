using Patches.Shared.Dtos;
using Spectre.Console;

namespace Patches.CLI;


public partial class PatchesCLI
{
    private async Task RenderLoadPatchScreenAsync()
    {
        PatchListItemDto? selectedPatch;
        var result = await ListPatchesHandler.HandleAsync(new());

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
