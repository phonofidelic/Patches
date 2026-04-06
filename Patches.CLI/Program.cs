using Patches.Application.Commands;
using Microsoft.Extensions.DependencyInjection;
using Patches.Application.Contracts;
using Patches.Services;
using Patches.CLI;
using Patches.Application.Handlers;

var services = new ServiceCollection();

services.AddScoped<IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult>, InitializePatchMatrixHandler>();
services.AddScoped<IHandler<AddModuleCommand, AddModuleResult>, AddModuleHandler>();
services.AddSingleton<IConsoleUIService, ConsoleUIService>();
services.AddSingleton<PatchesCLI>();

var serviceProvider = services.BuildServiceProvider();

var patches = serviceProvider.GetRequiredService<PatchesCLI>();

await patches.InitAsync();