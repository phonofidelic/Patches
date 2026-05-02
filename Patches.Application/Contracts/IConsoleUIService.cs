namespace Patches.Application.Contracts;

public interface IConsoleUIService
{
    void Write(string message, bool omitFromBuffer = false);
    void WriteLine();
    void WriteLine(string message, bool omitFromBuffer = false);
    void WritePreviousBuffer();
    void WriteCurrentBuffer();
    void ClearBuffer();
    void WriteError(string message);
    string? ReadLine();
    ConsoleKeyInfo ReadKey(bool intercept = false);
    void Clear();
    void DisplayHelp();
    T GetInput<T>(string prompt, string errorMessage, T defaultValue = default!) where T : IParsable<T>;
    string GetStringInput(string prompt, string errorMessage, bool isRequired = true);
    int GetIntInput(string prompt, string errorMessage, bool isRequired = true);
    void AddCommand(IConsoleCommand command);
    void TextBottom();
    void TextMiddle(bool omitFromBuffer = false);
    void VerticalPadding(int lines);
    int CursorTop { get; set; }
    int WindowHeight { get; }
}

public interface IConsoleCommand
{
    void DisplayHelp();

}