using System.ComponentModel.DataAnnotations;
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
        string helpScreen =
@"Patches
        
    Commands:
    
    'add'   Add a new Module
    'list'  List all Modules
    
    'q' to Quit";

        WriteLine(helpScreen);
    }

    public T GetInput<T>(string prompt, string errorMessage, T? defaultValue = default!) where T : IParsable<T>
    {
        Write(prompt, omitFromBuffer: true);

        string? input = null;

        while (input == null)
        {
            input = Console.ReadLine();

            if (string.IsNullOrEmpty(input) && defaultValue != null)
                return defaultValue;

            if (T.TryParse(input, null, out T? parsedInput))
            {
                return parsedInput;
            }

            WriteError(errorMessage);
            WriteLine();
            input = null;
        }
        throw new Exception("Could not parse input");
    }

    public int GetIntInput(string prompt, string errorMessage, bool isRequired = true)
    {
        Write(prompt);
        
        int? input = null;

        while (input == null)
        {
            if (!int.TryParse(ReadLine(), out int parsedInput))
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
        string? input = ReadLine();
        while (string.IsNullOrWhiteSpace(input) && isRequired)
        {
            Clear();
            Write(Buffer.Previous);
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(errorMessage, omitFromBuffer: true);
            Console.ResetColor();
            Write(prompt, omitFromBuffer: true);
            input = ReadLine();
        }

        return input ?? "";
    }

    public void TextBottom()
    {
        var bufferHeight = Buffer.Current.Split("\n").ToList().Count;
        var padding = Console.WindowHeight;
        Console.CursorTop = padding;
        for (int i = 0; i < padding; i++)
        {
            Buffer.Add("\n");
        }
        // Debug($"WindowHeight: {Console.WindowHeight}");
        // Debug($"padding: {Console.WindowHeight}");
    }

    public void TextMiddle(bool omitFromBuffer = false)
    {
        var bufferHeight = Buffer.Current.Split("\n").ToList().Count;
        var padding = (Console.WindowHeight) / 2;
        Console.CursorTop = padding;
        if (!omitFromBuffer)
        {
            for (int i = 0; i < padding; i++)
            {
                Buffer.Add("\n");
            }
        }
    }

    public string? ReadLine() => Console.ReadLine();

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
            Buffer.Add(message + "\n");
    }

    public void WritePreviousBuffer()
    {
        Console.Write(Buffer.Previous);
        Buffer.Current = Buffer.Previous;
    }

    public void WriteCurrentBuffer()
    {
        Console.Write(Buffer.Current);
    }

    public void ClearBuffer()
    {
        Buffer.Current = string.Empty;
    }

    public void VerticalPadding(int lines)
    {
        Console.CursorTop = lines;
    }

    public void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteLine(message, omitFromBuffer: true);
        Console.ResetColor();
    }
    public void Debug(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        WriteLine($"[Debug]: {message}");
        Console.ResetColor();
    }
}


class TextBuffer
{
    public string Previous { get; set; } = string.Empty;
    public string Current { get; set; } = string.Empty;

    public void Add(string input) => Current += input;
}