using System;

namespace Patches.CLI.ConsoleUI;

public interface IConsoleUI
{
    void Menu(IEnumerable<MenuItem> menuItems);
}

public class MenuItem
{
    
}

public static class UI
{
    public static void Clear() => Console.Clear();
    public static void Help()
    {
        Console.WriteLine("Patches");
        Console.WriteLine("");
        Console.WriteLine("Commands:");
        Console.WriteLine("");
        Console.WriteLine("add module");
        Console.WriteLine("list modules");
    }

    public static string GetString(string prompt, string errorMessage, bool required = true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(input) && required)
        {
            Console.WriteLine(errorMessage);
            Console.Write(prompt);
            input = Console.ReadLine();
        }

        return input ?? "";
    }

    public static int GetInt(string prompt, string errorMessage)
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
}