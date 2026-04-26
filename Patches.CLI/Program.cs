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

var services = new ServiceCollection();

var dbDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "Patches");
Directory.CreateDirectory(dbDir);
var dbPath = Path.Combine(dbDir, "patches.db");

services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});

services.AddLogging();
services.AddScoped<IRepository<Module>, ModuleRepository>();
services.AddScoped<IRepository<Vendor>, VendorRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

services.AddScoped<IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult>, InitializePatchMatrixHandler>();
services.AddScoped<IHandler<AddModuleCommand, AddModuleResult>, AddModuleHandler>();
services.AddScoped<IHandler<ListModulesQuery, ListModulesQueryResult>, ListModulesHandler>();
services.AddScoped<IHandler<ImportModulesFromJsonCommand, ImportModulesFromJsonResult>, ImportModulesFromJsonHandler>();
services.AddScoped<IHandler<GetModulesForPatchMatrixQuery, GetModulesForPatchMatrixQueryResult>, GetModulesForPatchMatrixHandler>();

services.AddSingleton<IConsoleUIService, ConsoleUIService>();
services.AddSingleton<PatchesCLI>();

services.AddAutoMapper(config => config.AddProfile<MapperProfile>());
services.AddScoped<IModulargridApiClient, ModulargridApiClient>();

var serviceProvider = services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

var patches = serviceProvider.GetRequiredService<PatchesCLI>();

await patches.InitAsync();