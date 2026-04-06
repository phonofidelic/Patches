namespace Patches.Application.Contracts;

public interface IConsoleUIService
{
    void Write(string message, bool omitFromBuffer = false);
    void WriteLine();
    void WriteLine(string message, bool omitFromBuffer = false);
    ConsoleKeyInfo ReadKey(bool intercept = false);
    void Clear();
    void DisplayHelp();
    string GetStringInput(string prompt, string errorMessage, bool isRequired = true);
    int GetIntInput(string prompt, string errorMessage, bool isRequired = true);
    void AddCommand(IConsoleCommand command);
    void TextBottom();
    void TextMiddle();
    int CursorTop { get; set; }
    int WindowHeight { get; }
}

public interface IConsoleCommand
{
    void DisplayHelp();

}