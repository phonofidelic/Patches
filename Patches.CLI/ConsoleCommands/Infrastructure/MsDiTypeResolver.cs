using Spectre.Console.Cli;

namespace Patches.CLI.ConsoleCommands.Infrastructure;

public sealed class MsDiTypeResolver(IServiceProvider provider) : ITypeResolver
{
    public object? Resolve(Type? type) =>
        type is null ? null : provider.GetService(type);
}
