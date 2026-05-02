using Patches.CLI;

namespace Patches.Tests;

public class DbPathHelperTests
{
    [Fact]
    public void GetDbPath_NoArgs_ReturnsPlatformDefault()
    {
        var result = DbPathHelper.GetDbPath([]);

        Assert.Contains("Patches", result);
        Assert.EndsWith("patches.db", result);
    }

    [Fact]
    public void GetDbPath_WithDbPathArg_ReturnsCustomPath()
    {
        var result = DbPathHelper.GetDbPath(["--db-path", "/tmp/custom.db"]);

        Assert.Equal("/tmp/custom.db", result);
    }

    [Fact]
    public void GetDbPath_DbPathArgMissingValue_ReturnsPlatformDefault()
    {
        var result = DbPathHelper.GetDbPath(["--db-path"]);

        Assert.Contains("Patches", result);
        Assert.EndsWith("patches.db", result);
    }
}
