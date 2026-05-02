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
    IHandler<LoadPatchMatrixQuery, LoadPatchMatrixResult> getModulesHandler,
    IHandler<AddConnectionCommand, AddConnectionResult> addConnectionHandler,
    IHandler<DeleteConnectionCommand, DeleteConnectionResult> deleteConnectionHandler) : IScreen
{
    private string? ErrorMessage { get; set;}
    public Task<string?> RunAsync() => RunAsync(null);

    public async Task<string?> RunAsync(PatchListItemDto? selectedPatch)
    {
        ConsoleKeyInfo keyCommand;
        string? textCommand = null;
        var position = new TablePosition();
        do
        {
            var result = await getModulesHandler.HandleAsync(new LoadPatchMatrixQuery()
            {
                PatchId = selectedPatch?.Id
            });

            var modules = result.Modules;
            ConnectionDto[] connections = [.. result.Connections];
            List<int> connectedInputIds = [.. connections.Select(i => i.InputId)];
            List<int> connectedOutputIds = [.. connections.Select(i => i.OutputId)];

            IReadOnlyList<PatchMatrixConnectionPointDto> columnConnectionPoints = result.Inputs;
            IReadOnlyList<string> columnConnectionPointNames = [.. columnConnectionPoints.Select(i => i.Name)];

            IReadOnlyList<PatchMatrixConnectionPointDto> rowConnectionPoints = result.Outputs;
            IReadOnlyList<string> rowConnectionPointNames = [.. rowConnectionPoints.Select(i => i.Name)];

            string moduleHeaderName = "MODULE";
            string signalTypeHeader = "SIGNAL";
            string connectionTypeHeader = "CONN.";

            int maxColumnConnectionPointNameLength = columnConnectionPointNames
                .Select(c => c.Split(" ").First())
                .Max(n => n.Length);

            int maxModuleNameHeaderLength = modules
                .Select(m => m.Name)
                .Append(moduleHeaderName)
                .Max(s => s.Length);

            int maxRowSignalTypeHeaderNameLength = rowConnectionPointNames
                .Select(s => s.Split(" ").First())
                .Append(signalTypeHeader)
                .Max(s => s.Length);

            int maxRowConnectionTypeHeaderNameLength = rowConnectionPoints
                .Select(c => c.Name)
                .Append(connectionTypeHeader)
                .Max(s => s.Length);

            int columnCount = columnConnectionPoints.Count;
            int rowCount = rowConnectionPoints.Count;

            ui.Clear();

            var patchMatrix = new Table()
                .NoBorder()
                .AddColumn("");

            // Add column headers for module input labels
            foreach (var item in columnConnectionPoints.Select((input, index) => (input, index)))
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
                string style = position.Col == item.index ? "#000 on #FFF" : "#FFF";
                patchMatrix.AddColumn($"[{style}]{header}[/]");
            }

            patchMatrix.AddRow($"[#FFF]{moduleHeaderName.PadRight(maxModuleNameHeaderLength, '_')}|{signalTypeHeader.PadLeft(maxRowSignalTypeHeaderNameLength / 2).PadRight(maxRowSignalTypeHeaderNameLength)}|{connectionTypeHeader.PadLeft(maxRowConnectionTypeHeaderNameLength / 2).PadRight(maxRowSignalTypeHeaderNameLength)}[/]");
            patchMatrix.AddEmptyRow();

            // Add row headers for module output labels and column cell content
            foreach (var item in rowConnectionPoints.Select((output, index) => (output, index)))
            {
                string style = item.index == position.Row ? "#000 on #FFF" : "#FFF";

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

                for (int i = 0; i < columnCount; i++)
                {
                    var output = item.output;
                    var input = columnConnectionPoints[i];

                    bool hasPatch = connections.FirstOrDefault(c =>
                        c.OutputId == output.Id &&
                        c.InputId == input.Id &&
                        c.PatchId == selectedPatch?.Id) != null;

                    string cellContent = hasPatch ? "•" : " ";

                    var cell = position.Row == item.index && position.Col == i
                        ? $"[#000 on #FFD787][[{cellContent}]][/]"
                        : $"[#FFD787][[{cellContent}]][/]";

                    cells.Add(cell);
                }

                patchMatrix.AddRow([header, .. cells]);
            }

            if (selectedPatch != null)
            {
                ansiConsole.MarkupLine($"[#FFD787 bold]{selectedPatch.Name}[/]");
                ansiConsole.MarkupLine($"[#FFD787]\n{selectedPatch.Description}[/]");
            }

            ansiConsole.Write(patchMatrix);

            ansiConsole.WriteLine("");
            var contextHelpRows = new Rows(
                new Text(""),
                new Markup("Use the [#FFD787 bold]arrow keys[/] to move the cursor across the patch matrix."),
                new Markup("Use the [#FFD787 bold]Space bar[/] to patch/un-patch a connection."),
                new Markup("Use [#FFD787 bold]Enter[/] to start typing a text command."),
                new Markup("Use [#FFD787 bold]/[/] to search for an input or output."),
                new Text(""),
                new Markup("Press [#FFD787 bold]Escape[/] return to the home screen."),
                new Text(""),
                new Markup(ErrorMessage ?? "")
            );
            
            ansiConsole.Write(contextHelpRows);

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
                    textCommand = ansiConsole.Prompt(
                        new TextPrompt<string?>($"[#FFD787]>[/]")
                            .AllowEmpty())
                        ?.Trim();
                    break;

                default:
                    if (keyCommand.KeyChar == '/')
                    {
                        var searchItems = columnConnectionPoints
                            .Select((cp, i) => new SearchResult($"[[INPUT]]  {cp.ModuleName}: {cp.Name}", i, true))
                            .Concat(rowConnectionPoints
                                .Select((cp, i) => new SearchResult($"[[OUTPUT]] {cp.ModuleName}: {cp.Name}", i, false)))
                            .ToList();

                        var searchPrompt = new SelectionPrompt<SearchResult>()
                            .Title("[#FFD787]Search:[/]")
                            .EnableSearch()
                            .HighlightStyle(new Style(Color.LightGoldenrod2_2, Console.BackgroundColor, Decoration.Bold))
                            .UseConverter(r => r.Label)
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

    private string? HandleUnknownCommand(string? command) {
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
