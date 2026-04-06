namespace Patches.Application.Contracts;

public interface IConsoleUIService
{
    void Write(string message);
    void WriteLine();
    void WriteLine(string message);
    void Clear();
    void DisplayHelp();
    string GetStringInput(string prompt, string errorMessage, bool isRequired = true);
    int GetIntInput(string prompt, string errorMessage, bool isRequired = true);
    void AddCommand(IConsoleCommand command);
}

public interface IConsoleCommand
{
    void DisplayHelp();

}