using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Patches.Domain.ValueObjects;

namespace Patches.Infrastructure.Data.Configurations;

public class ConnectionPointTypeConfiguration : IEntityTypeConfiguration<ConnectionPointType>
{
    public void Configure(EntityTypeBuilder<ConnectionPointType> builder)
    {
        builder.HasNoKey();
    }
}
