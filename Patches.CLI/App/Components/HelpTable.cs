using System;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Patches.CLI.App;

public class HelpTable : IRenderable
{
    private Table _table { get; set; }

    public HelpTable()
    {
        _table = new Table()
            .RoundedBorder()
            .AddColumns(["Commands", "Description"])
            .AddRow("[bold #FFD787]add[/]", "Add a new module")
            .AddEmptyRow()
            .AddRow("[bold #FFD787]import-json[/]", "Import modules from pasted Modulargrid JSON")
            .AddEmptyRow()
            .AddRow("[bold #FFD787]list[/]", "List all modules")
            .AddEmptyRow()
            .AddRow("[bold #FFD787]help[/]", "Display available commands")
            .AddEmptyRow()
            .AddRow("[bold #FFD787]quit[/]", "Quit Patches");
    }

    public int Height { get => _table.Rows.Count; }

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        
        return new Measurement();
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        foreach (var segment in _table.GetSegments(AnsiConsole.Console))
        {
            yield return segment;
        };
    }
}
