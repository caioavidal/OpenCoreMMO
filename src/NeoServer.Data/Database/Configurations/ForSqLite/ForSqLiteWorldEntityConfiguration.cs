﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Entities;
using NeoServer.Infrastructure.Database.Seeds;

namespace NeoServer.Infrastructure.Database.Configurations.ForSqLite;

public class ForSqLiteWorldEntityConfiguration : IEntityTypeConfiguration<WorldEntity>
{
    public void Configure(EntityTypeBuilder<WorldEntity> builder)
    {
        builder.ToTable("World");

        builder.HasKey(e => new { e.Id });

        builder.Property(e => e.Id).HasAnnotation("Sqlite:Autoincrement", true);
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Ip).IsRequired();

        WorldModelSeed.Seed(builder);
    }
}