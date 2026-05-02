namespace Patches.CLI;

public static class DbPathHelper
{
    public static string GetDbPath(string[] args)
    {
        var idx = Array.IndexOf(args, "--db-path");
        if (idx >= 0 && idx + 1 < args.Length)
            return args[idx + 1];

        var dbDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Patches");
        return Path.Combine(dbDir, "patches.db");
    }
}
