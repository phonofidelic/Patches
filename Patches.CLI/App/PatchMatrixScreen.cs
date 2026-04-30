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
    public Task<string?> RunAsync() => RunAsync(null);

    public async Task<string?> RunAsync(PatchListItemDto? selectedPatch)
    {
        ConsoleKeyInfo command;
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
            ansiConsole.WriteLine("Press Escape to exit");

            command = ui.ReadKey(intercept: true);
            switch (command.Key)
            {
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
            }

        } while (command.Key != ConsoleKey.Escape);

        return null;
    }

    private class TablePosition
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public void MoveRight() => Col++;
        public void MoveLeft() => Col--;
        public void MoveDown() => Row++;
        public void MoveUp() => Row--;
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
            Id = input.Id, Name = input.Name,
            ModuleId = input.ModuleId, Type = input.Type.Name
        };
        var outputDto = new ConnectionPointDto
        {
            Id = output.Id, Name = output.Name,
            ModuleId = output.ModuleId, Type = output.Type.Name
        };
        var result = await addConnectionHandler.HandleAsync(
            new AddConnectionCommand(inputDto, outputDto, patchId ?? -1), ct);
        return result.Connection != null
            ? [.. connections, result.Connection]
            : connections;
    }
}
