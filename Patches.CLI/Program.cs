using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Patches.CLI;
using Patches.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Patches.Domain.Entities;
using Patches.CLI.Extensions;

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

var patches = serviceProvider.GetRequiredService<PatchesCLI>();

await patches.InitAsync(dbPath);
