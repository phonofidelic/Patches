using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Patches.Application.Contracts;
using Patches.Services;
using Patches.CLI;
using Patches.Application.Handlers;
using Patches.Infrastructure.Data;
using Patches.Shared.Commands;
using Microsoft.EntityFrameworkCore;
using Patches.Infrastructure.Repositories;
using Patches.Domain.Entities;
using Patches.Shared.Queries;
using Patches.Infrastructure.ModulargridApi;
using Spectre.Console;
using Patches.CLI.App;

var services = new ServiceCollection();

var dbPath = DbPathHelper.GetDbPath(args);
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});

services.AddLogging();
services.AddScoped<IRepository<Module, Guid>, ModuleRepository>();
services.AddScoped<IRepository<Vendor, Guid>, VendorRepository>();
services.AddScoped<IRepository<ConnectionPoint, int>, ConnectionPointRepository>();
services.AddScoped<IConnectionRepository, ConnectionRepository>();
services.AddScoped<IRepository<Patch, int>, PatchRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

services.AddScoped<IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult>, InitializePatchMatrixHandler>();
services.AddScoped<IHandler<AddModuleCommand, AddModuleResult>, AddModuleHandler>();
services.AddScoped<IHandler<ListModulesQuery, ListModulesQueryResult>, ListModulesHandler>();
services.AddScoped<IHandler<ImportModulesFromJsonCommand, ImportModulesFromJsonResult>, ImportModulesFromJsonHandler>();
services.AddScoped<IHandler<LoadPatchMatrixQuery, LoadPatchMatrixResult>, LoadPatchMatrixHandler>();
services.AddScoped<IHandler<AddConnectionCommand, AddConnectionResult>, AddConnectionHandler>();
services.AddScoped<IHandler<DeleteConnectionCommand, DeleteConnectionResult>, DeleteConnectionHandler>();
services.AddScoped<IHandler<ListPatchesQuery, ListPatchesQueryResult>, ListPatchesQueryHandler>();

services.AddSingleton((sp) => AnsiConsole.Create(new AnsiConsoleSettings()));
services.AddSingleton<IConsoleUIService, ConsoleUIService>();
services.AddSingleton<HelpScreen>();
services.AddScoped<ModulesList>();
services.AddScoped<AddModuleForm>();
services.AddScoped<ImportModulesFromJsonForm>();
services.AddScoped<PatchMatrixScreen>();
services.AddScoped<LoadPatchScreen>();
services.AddSingleton<PatchesCLI>();

services.AddAutoMapper(config => config.AddProfile<MapperProfile>());
services.AddScoped<IModulargridApiClient, ModulargridApiClient>();

var serviceProvider = services.BuildServiceProvider();

#if DEBUG
serviceProvider.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
#endif

using (var scope = serviceProvider.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    if (!await db.ConnectionPointTypes.AnyAsync())
    {
        var input = new ConnectionPointType("Input");
        var output = new ConnectionPointType("Output");
        db.ConnectionPointTypes.AddRange([input, output]);
        await db.SaveChangesAsync();
    }
}

var patches = serviceProvider.GetRequiredService<PatchesCLI>();

await patches.InitAsync(dbPath);
