using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Patches.CLI.ConsoleCommands.Infrastructure;

public sealed class MsDiTypeRegistrar(IServiceCollection services) : ITypeRegistrar
{
    private IServiceProvider? _provider;

    public ITypeResolver Build()
    {
        _provider = services.BuildServiceProvider();
        return new MsDiTypeResolver(_provider);
    }

    public void Register(Type service, Type implementation) =>
        services.AddSingleton(service, implementation);

    public void RegisterInstance(Type service, object implementation) =>
        services.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory) =>
        services.AddSingleton(service, _ => factory());
}
