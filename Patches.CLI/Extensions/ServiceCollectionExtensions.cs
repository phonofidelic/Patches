using Microsoft.Extensions.DependencyInjection;
using Patches.Application.Contracts;
using Patches.Application.Handlers;
using Patches.CLI.App;
using Patches.Domain.Entities;
using Patches.Infrastructure.ModulargridApi;
using Patches.Infrastructure.Repositories;
using Patches.Services;
using Patches.Shared.Commands;
using Patches.Shared.Queries;
using Spectre.Console;

namespace Patches.CLI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Module, Guid>, ModuleRepository>();
        services.AddScoped<IRepository<Vendor, Guid>, VendorRepository>();
        services.AddScoped<IRepository<ConnectionPoint, int>, ConnectionPointRepository>();
        services.AddScoped<IConnectionRepository, ConnectionRepository>();
        services.AddScoped<IRepository<Patch, int>, PatchRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<AddModuleCommand, AddModuleResult>, AddModuleHandler>();
        services.AddScoped<IHandler<ListModulesQuery, ListModulesQueryResult>, ListModulesHandler>();
        services.AddScoped<IHandler<ImportModulesFromJsonCommand, ImportModulesFromJsonResult>, ImportModulesFromJsonHandler>();
        services.AddScoped<IHandler<LoadPatchMatrixQuery, LoadPatchMatrixResult>, LoadPatchMatrixHandler>();
        services.AddScoped<IHandler<AddConnectionCommand, AddConnectionResult>, AddConnectionHandler>();
        services.AddScoped<IHandler<DeleteConnectionCommand, DeleteConnectionResult>, DeleteConnectionHandler>();
        services.AddScoped<IHandler<ListPatchesQuery, ListPatchesQueryResult>, ListPatchesQueryHandler>();
        return services;
    }

    public static IServiceCollection AddConsoleApp(this IServiceCollection services)
    {
        services.AddSingleton<IConsoleUIService, ConsoleUIService>();
        services.AddSingleton<IModulargridApiClient, ModulargridApiClient>();
        services.AddSingleton((sp) => AnsiConsole.Create(new AnsiConsoleSettings()));
        services.AddSingleton<HelpScreen>();
        services.AddSingleton<ModulesList>();
        services.AddSingleton<AddModuleForm>();
        services.AddSingleton<ImportModulesFromJsonForm>();
        services.AddSingleton<PatchMatrixScreen>();
        services.AddSingleton<LoadPatchScreen>();
        services.AddSingleton<PatchesCLI>();
        return services;
    }
}
