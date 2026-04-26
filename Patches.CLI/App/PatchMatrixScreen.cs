using System;
using System.Drawing;
using Patches.Shared.Commands;
using Patches.Shared.Dtos;
using Patches.Shared.Queries;
using Spectre.Console;

namespace Patches.CLI;

public class TablePosition()
{
    public int Row { get; private set; }
    public int Col { get; private set; }

    public void MoveRight() => Col++;
    public void MoveLeft() => Col--;
    public void MoveDown() => Row++;
    public void MoveUp() => Row--;
}
public partial class PatchesCLI
{
    public async Task RenderPatchMatrixScreenAsync()
    {
        var result = await GetModulesForPatchMatrixHandler.HandleAsync(new GetModulesForPatchMatrixQuery());
        var modules = result.Modules;
        ConsoleKeyInfo command;
        var position = new TablePosition();

        var random = new Random();
        // Add mock connection points
        foreach (var module in modules)
        {
            module.ConnectionPoints.Add(new()
            {
                Id = random.Next(),
                Name = "CV In",
                ModuleName = module.Name,
                ModuleId = module.Id,
                Type = PatchMatrixConnectionPointType.Input
            });
            module.ConnectionPoints.Add(new()
            {
                Id = random.Next(),
                Name = "Gate In",
                ModuleName = module.Name,
                ModuleId = module.Id,
                Type = PatchMatrixConnectionPointType.Input
            });
            module.ConnectionPoints.Add(new()
            {
                Id = random.Next(),
                Name = "CV Out",
                ModuleName = module.Name,
                ModuleId = module.Id,
                Type = PatchMatrixConnectionPointType.Output
            });
            module.ConnectionPoints.Add(new()
            {
                Id = random.Next(),
                Name = "Gate Out",
                ModuleName = module.Name,
                ModuleId = module.Id,
                Type = PatchMatrixConnectionPointType.Output
            });
        }

        List<PatchMatrixConnectionPointDto> columnConnectionPoints = [];
        List<string> columnConnectionPointNames = [];

        List<PatchMatrixConnectionPointDto> rowConnectionPoints = [];
        List<string> rowConnectionPointNames = [];

        foreach (var module in modules)
        {
            columnConnectionPoints = [
                ..columnConnectionPoints, 
                ..module.ConnectionPoints
                    .Where(c => 
                        c.Type == PatchMatrixConnectionPointType.Input ||
                        c.Type == PatchMatrixConnectionPointType.Multiple)
                    .ToList()];

            columnConnectionPointNames = [
                ..columnConnectionPointNames, 
                ..columnConnectionPoints.Select(i => i.Name).ToList()];

            rowConnectionPoints = [
                ..rowConnectionPoints, 
                ..module.ConnectionPoints
                    .Where(c => 
                        c.Type == PatchMatrixConnectionPointType.Output ||
                        c.Type == PatchMatrixConnectionPointType.Multiple)
                    .ToList()];

            rowConnectionPointNames = [
                ..rowConnectionPointNames,
                ..rowConnectionPoints.Select(i => i.Name).ToList()];
        };

        string moduleHeaderName = "MODULE";
        string signalTypeHeader = "SIGNAL";
        string connectionTypeHeader = "CONN.";

        int maxColumnConnectionPointNameLength = columnConnectionPointNames.Max(n => n.Length);
        
        int maxModuleNameHeaderLength = modules
            .Select(m => m.Name)
            .Append(moduleHeaderName)
            .Max(s => s.Length);
        int maxRowSignalTypeHeaderNameLength = rowConnectionPointNames
            .Append(signalTypeHeader)
            .Max(s => s.Length);
        int maxRowConnectionTypeHeaderNameLength = rowConnectionPoints
            .Select(c => c.Name)
            .Append(connectionTypeHeader)
            .Max(s => s.Length);
        
        int columnCount = modules
            .Sum(m => m.ConnectionPoints
                .Count(c => 
                    c.Type == PatchMatrixConnectionPointType.Input ||
                    c.Type == PatchMatrixConnectionPointType.Multiple));
        
        int rowCount = modules
            .Sum(m => m.ConnectionPoints
                .Count(
                    c => c.Type == PatchMatrixConnectionPointType.Output ||
                    c.Type == PatchMatrixConnectionPointType.Multiple));

        // Render loop
        do
        {
            UI.Clear();

            var patchMatrix = new Table()
                .NoBorder()
                .AddColumn("");

            foreach (var item in columnConnectionPoints.Select((input, index) => (input, index)))
            {
                string header = string.Join("\n", [
                        ..item.input.ModuleName.PadRight(5).Take(5).ToArray().Select(c => $" {c} "),
                        " - ",
                        ..item.input.Name.ToUpper()
                            .PadLeft((item.input.Name.Length + maxColumnConnectionPointNameLength)/2)
                            .PadRight(maxColumnConnectionPointNameLength)
                            .ToArray()
                            .Select(c => $" {c} "),
                        " - ",
                        ..item.input.Type.ToString().ToUpper().ToArray().Take(2).Select(c => $" {c} ")]);
                    string style = position.Col == item.index ? "#000 on #FFF" : "#FFF";
                    patchMatrix.AddColumn($"[{style}]{header}[/]");
            }

            
            patchMatrix.AddRow($"[#FFF]{moduleHeaderName.PadRight(maxModuleNameHeaderLength, '_')}|{signalTypeHeader.PadLeft(maxRowSignalTypeHeaderNameLength / 2).PadRight(maxRowSignalTypeHeaderNameLength)}|{connectionTypeHeader.PadLeft(maxRowConnectionTypeHeaderNameLength / 2).PadRight(maxRowSignalTypeHeaderNameLength)}[/]");
            patchMatrix.AddEmptyRow();

            foreach(var item in rowConnectionPoints.Select((output, index) => (output, index)))
            {
                string style = item.index == position.Row ? "#000 on #FFF" : "#FFF";
                string signalType = item.output.Name.ToUpper()
                    .PadLeft((item.output.Name.Length + maxRowSignalTypeHeaderNameLength) / 2)
                    .PadRight(maxRowSignalTypeHeaderNameLength);
                string connectionType = item.output.Type
                    .ToString()
                    .ToUpper()[..3]
                    .PadLeft((3 + maxRowSignalTypeHeaderNameLength) / 2)
                    .PadRight(maxRowSignalTypeHeaderNameLength);
                string header = $"[{style}]{item.output.ModuleName.PadRight(maxModuleNameHeaderLength, '_')}|{signalType}|{connectionType}[/]";
                List<string> cells = [];
                
                for (int i = 0; i < columnCount; i++)
                {
                    var outputId = rowConnectionPoints[position.Row].Id;
                    var inputId = columnConnectionPoints[position.Col].Id;
                    string cellContent = " ";
                    var cell = position.Row == item.index && position.Col == i 
                        ? $"[#000 on #FFD787][[{cellContent}]][/]"
                        : $"[#FFD787][[{cellContent}]][/]";

                    cells.Add(cell);
                }

                patchMatrix.AddRow([header, ..cells]);
            }

            AnsiConsole.Write(patchMatrix);

            AnsiConsole.WriteLine("");
            AnsiConsole.WriteLine("Press Escape to exit");

            command = UI.ReadKey(intercept: true);
            switch(command.Key)
            {
                case ConsoleKey.RightArrow:
                    if (position.Col < columnConnectionPoints.Count -1)
                        position.MoveRight();
                    break;

                case ConsoleKey.LeftArrow:
                    if (position.Col > 0)
                        position.MoveLeft();
                    break;

                case ConsoleKey.DownArrow:
                    if (position.Row < rowConnectionPoints.Count -1)
                        position.MoveDown();
                    break;

                case ConsoleKey.UpArrow:
                    if (position.Row > 0)
                        position.MoveUp();
                    break;
                
                case ConsoleKey.Spacebar:
                    await AddPatchMatrixConnectionAsync(
                        input: columnConnectionPoints[position.Col],
                        output: rowConnectionPoints[position.Row]);
                    break;

            }
                
        } while (command.Key != ConsoleKey.Escape);
    }

    private async Task AddPatchMatrixConnectionAsync(
        PatchMatrixConnectionPointDto input,
        PatchMatrixConnectionPointDto output,
        CancellationToken ct = default)
    {
        var inputConnectionPointDto = new ConnectionPointDto
        {
            Id = input.Id,
            Name = input.Name,
            ModuleId = input.ModuleId,
            Type = input.Type.ToString()
        };

        var outputConnectionPointDto = new ConnectionPointDto
        {
            Id = output.Id,
            Name = output.Name,
            ModuleId = output.ModuleId,
            Type = output.Type.ToString()
        };

        var command = new AddConnectionCommand(
            input: inputConnectionPointDto,
            output: outputConnectionPointDto,
            -1
        );

        await AddConnectionHandler.HandleAsync(command, ct);
    }
}
