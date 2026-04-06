using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Patches.Domain.Entities;

namespace Patches.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): DbContext(options)
{
    public DbSet<Domain.Entities.Module> Modules { get; set; }
    public DbSet<ConnectionPoint> ConnectionPoints { get; set; }
    public DbSet<Vendor> Vendors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
