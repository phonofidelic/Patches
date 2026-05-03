using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Patches.CLI;
using Patches.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Patches.Domain.Entities;
using Patches.CLI.Extensions;
using Patches.CLI.ConsoleCommands;
using Patches.CLI.ConsoleCommands.Infrastructure;
using Spectre.Console.Cli;

var services = new ServiceCollection();

var dbPath = DbPathHelper.GetDbPath(args);
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});
services.AddLogging();
services.AddRepositories();
services.AddHandlers();
services.AddConsoleApp();
services.AddCommands();
services.AddAutoMapper(config => config.AddProfile<MapperProfile>());

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

var remainingArgs = DbPathHelper.StripDbPathArgs(args);

if (remainingArgs.Length == 0)
{
    var patches = serviceProvider.GetRequiredService<PatchesCLI>();
    await patches.InitAsync(dbPath);
}
else
{
    var registrar = new MsDiTypeRegistrar(services);
    var app = new CommandApp(registrar);
    app.Configure(config =>
    {
        config.AddCommand<AddModuleConsoleCommand>("add")
              .WithDescription("Add a new module");
        config.AddCommand<ListModulesConsoleCommand>("list")
              .WithDescription("List all modules");
        config.AddCommand<ImportModulesConsoleCommand>("import-json")
              .WithDescription("Import modules from a JSON file");
    });
    Environment.Exit(await app.RunAsync(remainingArgs));
}
