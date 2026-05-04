using Patches.Application.Contracts;
using Patches.CLI.App.Contracts;
using Patches.Shared.Commands;
using Patches.Shared.Dtos;
using Patches.Shared.Queries;
using Spectre.Console;

namespace Patches.CLI.App;

public class PatchMatrixScreen(
    IConsoleUIService ui,
    IAnsiConsole ansiConsole,
    IHandler<LoadPatchMatrixQuery, LoadPatchMatrixResult> loadPatchMatrixHandler,
    IHandler<AddConnectionCommand, AddConnectionResult> addConnectionHandler,
    IHandler<DeleteConnectionCommand, DeleteConnectionResult> deleteConnectionHandler) : IScreen
{
    private enum CommandMode
    {
        KeyCommand,
        TextCommand
    };

    private record RenderScreenDto(
        RenderPatchMatrixTableDto RenderPatchMatrixTableDto,
        RenderContextHelpDto RenderContextHelpDto,
        string? ErrorMessage
    );

    private record RenderPatchMatrixTableDto(
        PatchListItemDto? SelectedPatch,
        TablePosition Position,
        IEnumerable<string> ColumnConnectionPointNames,
        IEnumerable<string> RowConnectionPointNames,
        IReadOnlyList<PatchMatrixConnectionPointDto> RowConnectionPoints,
        IReadOnlyList<PatchMatrixConnectionPointDto> ColumnConnectionPoints,
        IEnumerable<PatchMatrixItemDto> Modules,
        ConnectionDto[] Connections);

    private record RenderContextHelpDto(
        CommandMode CurrentCommandMode
    );

    private string? ErrorMessage { get; set; }

    public Task<string?> RunAsync() => RunAsync(null);

    public async Task<string?> RunAsync(PatchListItemDto? selectedPatch)
    {
        ConsoleKeyInfo keyCommand;
        string? textCommand = null;
        var position = new TablePosition();
        do
        {
            var result = await loadPatchMatrixHandler.HandleAsync(new LoadPatchMatrixQuery()
            {
                PatchId = selectedPatch?.Id
            });

            ConnectionDto[] connections = [.. result.Connections];
            List<int> connectedInputIds = [.. connections.Select(i => i.InputId)];
            List<int> connectedOutputIds = [.. connections.Select(i => i.OutputId)];

            IReadOnlyList<PatchMatrixConnectionPointDto> columnConnectionPoints = result.Inputs;
            IReadOnlyList<string> columnConnectionPointNames = [.. columnConnectionPoints.Select(i => i.Name)];

            IReadOnlyList<PatchMatrixConnectionPointDto> rowConnectionPoints = result.Outputs;
            IReadOnlyList<string> rowConnectionPointNames = [.. rowConnectionPoints.Select(i => i.Name)];

            var modules = result.Modules;

            var renderPatchMatrixTablePropsDto = new RenderPatchMatrixTableDto(
                selectedPatch,
                position,
                columnConnectionPointNames,
                rowConnectionPointNames,
                rowConnectionPoints,
                columnConnectionPoints,
                modules,
                connections);

            RenderScreen(new(
                RenderPatchMatrixTableDto: renderPatchMatrixTablePropsDto,
                RenderContextHelpDto: new(CommandMode.KeyCommand),
                ErrorMessage
            ));

            ansiConsole.Cursor.SetPosition(0, Console.WindowHeight);
            keyCommand = ui.ReadKey(intercept: true);

            ErrorMessage = null;

            switch (keyCommand.Key)
            {
                case ConsoleKey.Escape:
                    break;

                case ConsoleKey.RightArrow:
                    if (position.Col < columnConnectionPoints.Count - 1)
                        position.MoveRight();
                    break;

                case ConsoleKey.LeftArrow:
                    if (position.Col > 0)
                        position.MoveLeft();
                    break;

                case ConsoleKey.DownArrow:
                    if (position.Row < rowConnectionPoints.Count - 1)
                        position.MoveDown();
                    break;

                case ConsoleKey.UpArrow:
                    if (position.Row > 0)
                        position.MoveUp();
                    break;

                case ConsoleKey.Spacebar:
                    connections = await ToggleConnectionAsync(
                        connections,
                        input: columnConnectionPoints[position.Col],
                        output: rowConnectionPoints[position.Row],
                        patchId: selectedPatch?.Id);
                    break;

                case ConsoleKey.Enter:
                    RenderScreen(new(
                        RenderPatchMatrixTableDto: renderPatchMatrixTablePropsDto,
                        RenderContextHelpDto: new(CommandMode.TextCommand),
                        ErrorMessage
                    ));

                    ansiConsole.Cursor.SetPosition(0, Console.WindowHeight);
                    textCommand = ansiConsole.Prompt(
                        new TextPrompt<string?>($"[#FFD787]>[/]")
                            .AllowEmpty())
                        ?.Trim();
                    break;

                default:
                    if (keyCommand.KeyChar == '/')
                    {
                        var searchItems = columnConnectionPoints
                            .Select((cp, i) => new SearchResult($"{cp.ModuleName}: {cp.Name}", i, true))
                            .Concat(rowConnectionPoints
                                .Select((cp, i) => new SearchResult($"{cp.ModuleName}: {cp.Name}", i, false)))
                            .OrderBy(sr => sr.Label)
                            .ToList();

                        var searchPrompt = new SelectionPrompt<SearchResult>()
                            .Title("[#FFD787]Search:[/]")
                            .EnableSearch()
                            .HighlightStyle(new Style(Color.LightGoldenrod2_2, Console.BackgroundColor, Decoration.Bold))
                            .UseConverter(r => $"{(r.IsInput ? "[[INPUT]] " : "[[OUTPUT]]")} {r.Label}")
                            .AddChoices(searchItems)
                            .AddCancelResult(new SearchResult("", -1, false));

                        var selected = ansiConsole.Prompt(searchPrompt);

                        // Break if search is canceled
                        if (selected.Index == -1)
                            break;

                        if (selected.IsInput)
                            position.MoveToCol(selected.Index);
                        else
                            position.MoveToRow(selected.Index);
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(textCommand))
                textCommand = textCommand switch
                {
                    _ => HandleUnknownCommand(textCommand)
                };
        } while (keyCommand.Key != ConsoleKey.Escape);

        return null;
    }

    private void RenderScreen(RenderScreenDto dto)
    {
        var patch = dto.RenderPatchMatrixTableDto.SelectedPatch;
        ui.Clear();
        if (patch != null)
        {
            ansiConsole.MarkupLine($"[#FFD787 bold]{patch.Name}[/]");
            ansiConsole.MarkupLine($"[#FFD787]\n{patch.Description}[/]");
        }
        ansiConsole.Write(RenderPatchMatrixTable(dto.RenderPatchMatrixTableDto));
        ansiConsole.WriteLine("");
        ansiConsole.Write(RenderContextHelp(dto.RenderContextHelpDto));
        ansiConsole.MarkupLine(dto.ErrorMessage ?? "");
    }

    private static Table RenderPatchMatrixTable(RenderPatchMatrixTableDto dto)
    {
        string moduleHeaderName = "MODULE";
        string signalTypeHeader = "SIGNAL";
        string connectionTypeHeader = "CONN.";

        int maxColumnConnectionPointNameLength = dto.ColumnConnectionPointNames
            .Select(c => c.Split(" ").First())
            .Max(n => n.Length);

        int maxModuleNameHeaderLength = dto.Modules
            .Select(m => m.Name)
            .Append(moduleHeaderName)
            .Max(s => s.Length);

        int maxRowSignalTypeHeaderNameLength = dto.RowConnectionPointNames
            .Select(s => s.Split(" ").First())
            .Append(signalTypeHeader)
            .Max(s => s.Length);

        int maxRowConnectionTypeHeaderNameLength = dto.RowConnectionPoints
            .Select(c => c.Name)
            .Append(connectionTypeHeader)
            .Max(s => s.Length);

        var patchMatrix = new Table()
            .NoBorder()
            .AddColumn("");

        // Add column headers for module input labels
        foreach (var item in dto.ColumnConnectionPoints.Select((input, index) => (input, index)))
        {
            string signalName = item.input.Name.ToUpper()
                        .Split(" ")
                        .First();
            string header = string.Join("\n", [
                    ..item.input.ModuleName.PadRight(5).Take(5).ToArray().Select(c => $" {c} "),
                    " - ",
                    ..signalName
                        .PadLeft((signalName.Length + maxColumnConnectionPointNameLength) / 2)
                        .PadRight(maxColumnConnectionPointNameLength)
                        .ToArray()
                        .Select(c => $" {c} "),
                    " - ",
                    ..item.input.Type.Name.ToUpper().ToArray().Take(2).Select(c => $" {c} ")]);
            string style = dto.Position.Col == item.index ? "#000 on #FFF" : "#FFF";
            patchMatrix.AddColumn($"[{style}]{header}[/]");
        }

        patchMatrix.AddRow($"[#FFF]{moduleHeaderName.PadRight(maxModuleNameHeaderLength, '_')}|{signalTypeHeader.PadLeft(maxRowSignalTypeHeaderNameLength / 2).PadRight(maxRowSignalTypeHeaderNameLength)}|{connectionTypeHeader.PadLeft(maxRowConnectionTypeHeaderNameLength / 2).PadRight(maxRowSignalTypeHeaderNameLength)}[/]");
        patchMatrix.AddEmptyRow();

        // Add row headers for module output labels and column cell content
        foreach (var item in dto.RowConnectionPoints.Select((output, index) => (output, index)))
        {
            string style = item.index == dto.Position.Row ? "#000 on #FFF" : "#FFF";

            string signalName = item.output.Name
                .ToUpper()
                .Split(" ")
                .First();

            string signalType = signalName
                .PadLeft((signalName.Length + maxRowSignalTypeHeaderNameLength) / 2)
                .PadRight(maxRowSignalTypeHeaderNameLength);
            string connectionType = item.output.Type.Name
                .ToUpper()[..3]
                .PadLeft((3 + maxRowSignalTypeHeaderNameLength) / 2)
                .PadRight(maxRowSignalTypeHeaderNameLength);
            string header = $"[{style}]{item.output.ModuleName.PadRight(maxModuleNameHeaderLength, '_')}|{signalType}|{connectionType}[/]";
            List<string> cells = [];

            for (int i = 0; i < dto.ColumnConnectionPoints.Count; i++)
            {
                var output = item.output;
                var input = dto.ColumnConnectionPoints[i];

                bool hasPatch = dto.Connections.FirstOrDefault(c =>
                    c.OutputId == output.Id &&
                    c.InputId == input.Id &&
                    c.PatchId == dto.SelectedPatch?.Id) != null;

                string cellContent = hasPatch ? "•" : " ";

                var cell = dto.Position.Row == item.index && dto.Position.Col == i
                    ? $"[#000 on #FFD787][[{cellContent}]][/]"
                    : $"[#FFD787][[{cellContent}]][/]";

                cells.Add(cell);
            }

            patchMatrix.AddRow([header, .. cells]);
        }

        return patchMatrix;
    }

    private string? HandleUnknownCommand(string? command)
    {
        ErrorMessage = $"[#FF5F5F]Unknown command: '{Markup.Escape(command ?? "")}'[/]";
        return null;
    }

    private record SearchResult(string Label, int Index, bool IsInput);

    private class TablePosition
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public void MoveRight() => Col++;
        public void MoveLeft() => Col--;
        public void MoveDown() => Row++;
        public void MoveUp() => Row--;
        public void MoveToRow(int row) => Row = row;
        public void MoveToCol(int col) => Col = col;
    }

    private static Rows RenderContextHelp(RenderContextHelpDto dto)
    {
        Markup[] keyCommandHelpRows = [
            new Markup("Use the [#FFD787 bold]arrow keys[/] to move the cursor across the patch matrix."),
            new Markup("Use the [#FFD787 bold]Space bar[/] to patch/un-patch a connection."),
            new Markup("Use [#FFD787 bold]/[/] to search for an input or output."),
            new Markup("Use [#FFD787 bold]Enter[/] to start typing a text command."),
            new Markup(" "),
            new Markup("Press [#FFD787 bold]Escape[/] return to the home screen."),
        ];

        Markup[] textCommandHelpRows = [
            new Markup("[#FFD787 bold]Enter[/] an empty line to return to key commands.")
        ];

        return new Rows([
            new Text(""),
            .. dto.CurrentCommandMode == CommandMode.KeyCommand ? keyCommandHelpRows : textCommandHelpRows,
            new Text(""),
        ]);
    }

    private async Task<ConnectionDto[]> ToggleConnectionAsync(
        ConnectionDto[] connections,
        PatchMatrixConnectionPointDto input,
        PatchMatrixConnectionPointDto output,
        int? patchId,
        CancellationToken ct = default)
    {
        var existing = connections.FirstOrDefault(c =>
            c.InputId == input.Id &&
            c.OutputId == output.Id &&
            c.PatchId == patchId);

        if (existing != null)
        {
            await deleteConnectionHandler.HandleAsync(
                new DeleteConnectionCommand(existing.PatchId, existing.InputId, existing.OutputId), ct);
            return [.. connections.Where(c =>
                !(c.PatchId == existing.PatchId &&
                  c.InputId == existing.InputId &&
                  c.OutputId == existing.OutputId))];
        }

        var inputDto = new ConnectionPointDto
        {
            Id = input.Id,
            Name = input.Name,
            ModuleId = input.ModuleId,
            Type = input.Type.Name
        };
        var outputDto = new ConnectionPointDto
        {
            Id = output.Id,
            Name = output.Name,
            ModuleId = output.ModuleId,
            Type = output.Type.Name
        };
        var result = await addConnectionHandler.HandleAsync(
            new AddConnectionCommand(inputDto, outputDto, patchId ?? -1), ct);
        return result.Connection != null
            ? [.. connections, result.Connection]
            : connections;
    }
}
