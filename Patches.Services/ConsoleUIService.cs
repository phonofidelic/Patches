using Patches.Application.Contracts;

namespace Patches.Services;

public class ConsoleUIService : IConsoleUIService
{
    public int CursorTop { get => Console.CursorTop; set => Console.CursorTop = value; }
    public int WindowHeight { get => Console.WindowHeight; }

    private TextBuffer Buffer { get; set; } = new();

    public void AddCommand(IConsoleCommand command)
    {
        throw new NotImplementedException();
    }

    public void Clear() { 
        Buffer.Previous = Buffer.Current;
        Console.Clear();
        Buffer.Current = string.Empty;
    }

    public void DisplayHelp()
    {
        string helpScreen = @"Patches
        
        Commands:
        
        'add'   Add a new Module
        'list'  List all Modules
        
        'q' to Quit";
        // Console.WriteLine("Patches");
        // Console.WriteLine("");
        // Console.WriteLine("Commands:");
        // Console.WriteLine("");
        // Console.WriteLine("add module");
        // Console.WriteLine("list modules");
        WriteLine(helpScreen);
    }

    public int GetIntInput(string prompt, string errorMessage, bool isRequired = true)
    {
        Write(prompt);
        
        int? input = null;

        while (input == null)
        {
            if (!int.TryParse(Console.ReadLine(), out int parsedInput))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                WriteLine(errorMessage);
                Console.ResetColor();
                WriteLine(prompt);
            } else
            {
                input = parsedInput;
            }
        }

        return input ?? throw new Exception(errorMessage);
    }

    public string GetStringInput(string prompt, string errorMessage, bool isRequired = true)
    {
        Clear();
        Write(Buffer.Previous);
        Write(prompt, omitFromBuffer: true);
        string? input = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(input) && isRequired)
        {
            Clear();
            Write(Buffer.Previous);
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(errorMessage, omitFromBuffer: true);
            Console.ResetColor();
            Write(prompt, omitFromBuffer: true);
            input = Console.ReadLine();
        }

        return input ?? "";
    }

    public void TextBottom()
    {
        var padding = Console.WindowHeight - 2;
        Console.CursorTop = padding;
        for (int i = 0; i < padding; i++)
        {
            Buffer.Add("\n");
        }
    }

    public void TextMiddle()
    {
        var padding = (Console.WindowHeight / 2) - 2;
        Console.CursorTop = padding;
        for (int i = 0; i < padding; i++)
        {
            Buffer.Add("\n");
        }
    }

    public ConsoleKeyInfo ReadKey(bool intercept = false) => Console.ReadKey(intercept);

    public void Write(string message, bool omitFromBuffer = false) {
        Console.Write(message);
        if (!omitFromBuffer)
            Buffer.Add(message);
    }

    public void WriteLine() { 
        Console.WriteLine();
        Buffer.Add("\n");
    }

    public void WriteLine(string message, bool omitFromBuffer = false) {
        Console.WriteLine(message);
        if (!omitFromBuffer)
            Buffer.Add("\n" + message);
    }
}

class TextBuffer
{
    public string Previous { get; set; } = string.Empty;
    public string Current { get; set; } = string.Empty;

    public void Add(string input) => Current += input;
}