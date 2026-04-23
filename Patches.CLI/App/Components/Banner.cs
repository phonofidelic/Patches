using Spectre.Console;
using Spectre.Console.Rendering;

namespace Patches.CLI.App;

public class Banner(string title, string? subtitle = null) : IRenderable
{
    private string Title { get; init; } = title;
    private string? Subtitle { get; init; } = subtitle;
    private FigletFont Font { get; set; } = FigletFont.Load("Fonts/isometric1.flf");

    public int Height { get => Console.WindowHeight > 38 ? Font.Height + (Subtitle != null ? 1 : 0) : 1;}
    
    public Measurement Measure(RenderOptions options, int maxWidth) =>
        new (Title.Length + Subtitle?.Length ?? 0 + 1, Font.MaxWidth * Title.Length);
    
    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        IRenderable renderable = Console.WindowHeight > 38 
            ? GetLargeRenderable()
            : GetSmallRenderable();

        foreach (var segment in renderable.GetSegments(AnsiConsole.Console))
        {
            yield return segment;
        }
    }

    private IRenderable GetLargeRenderable()
    {
        var banner = new FigletText(Font, Title)
            {
                Color = Color.LightGoldenrod2_2,
            };
        
        if (Subtitle == null)
            return banner;

        return new Rows(
            banner,
            new Markup($"[#FFD787]{Subtitle}[/]")
        );
    }

    private IRenderable GetSmallRenderable() => 
        new Markup($"[#FFD787][bold]{Title.ToUpper()}[/] {Subtitle}[/]");
}
