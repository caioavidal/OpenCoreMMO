using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Entities;

namespace NeoServer.Infrastructure.Database.Configurations;

public class GuildRankEntityConfiguration : IEntityTypeConfiguration<GuildRankEntity>
{
    public void Configure(EntityTypeBuilder<GuildRankEntity> builder)
    {
        builder.ToTable("GuildRank");

        builder.HasKey(e => new { e.Id });

        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.GuildId);
        builder.Property(e => e.Name);
        builder.Property(e => e.Level);
    }
}