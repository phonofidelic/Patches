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

var services = new ServiceCollection();

services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("Patches.InMemoryDb");
});

services.AddLogging();
services.AddScoped<IRepository<Module>, ModuleRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

services.AddScoped<IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult>, InitializePatchMatrixHandler>();
services.AddScoped<IHandler<AddModuleCommand, AddModuleResult>, AddModuleHandler>();
services.AddSingleton<IConsoleUIService, ConsoleUIService>();
services.AddSingleton<PatchesCLI>();

services.AddAutoMapper(config => config.AddProfile<MapperProfile>());

var serviceProvider = services.BuildServiceProvider();

var patches = serviceProvider.GetRequiredService<PatchesCLI>();

await patches.InitAsync();