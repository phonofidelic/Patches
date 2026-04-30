using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Patches.Domain.Entities;

namespace Patches.Infrastructure.Data.Configurations;

public class ConnectionConfiguration : IEntityTypeConfiguration<Connection>
{
    public void Configure(EntityTypeBuilder<Connection> builder)
    {
        builder.HasOne(c => c.Patch)
            .WithMany(p => p.Connections);

        builder.HasOne(c => c.Input);
        
        builder.HasOne(c => c.Output);

        builder
            .HasKey(c => new { c.PatchId, c.InputId, c.OutputId });
    }
}
