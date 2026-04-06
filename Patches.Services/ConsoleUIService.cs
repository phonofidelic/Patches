using Patches.Application.Contracts;

namespace Patches.Services;

public class ConsoleUIService : IConsoleUIService
{
    public void AddCommand(IConsoleCommand command)
    {
        throw new NotImplementedException();
    }

    public void Clear() => Console.Clear();

    public void DisplayHelp()
    {
        Console.WriteLine("Patches");
        Console.WriteLine("");
        Console.WriteLine("Commands:");
        Console.WriteLine("");
        Console.WriteLine("add module");
        Console.WriteLine("list modules");
    }

    public int GetIntInput(string prompt, string errorMessage, bool isRequired = true)
    {
        Console.Write(prompt);
        int? input = null;

        while (input == null)
        {
            if (!int.TryParse(Console.ReadLine(), out int parsedInput))
            {
                Console.WriteLine(errorMessage);
                Console.WriteLine(prompt);
            } else
            {
                input = parsedInput;
            }
        }

        return input ?? throw new Exception(errorMessage);
    }

    public string GetStringInput(string prompt, string errorMessage, bool isRequired = true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(input) && isRequired)
        {
            Console.WriteLine(errorMessage);
            Console.Write(prompt);
            input = Console.ReadLine();
        }

        return input ?? "";
    }

    public void Write(string message) => Console.Write(message);

    public void WriteLine() => Console.WriteLine();

    public void WriteLine(string message) => Console.WriteLine(message);
}
