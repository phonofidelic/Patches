using System;

namespace Patches.CLI.App.Contracts;

public interface IScreen
{
    Task<string?> RunAsync();
}
