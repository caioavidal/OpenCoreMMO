using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Entities;

namespace NeoServer.Data.Configurations;

public class PlayerOutfitAddonModelConfiguration : IEntityTypeConfiguration<PlayerOutfitAddonEntity>
{
    public void Configure(EntityTypeBuilder<PlayerOutfitAddonEntity> builder)
    {
        builder.ToTable("player_outfit_addon");

        builder.HasKey(e => new { e.PlayerId, e.LookType, e.AddonLevel });

        builder.Property(e => e.LookType).HasColumnName("look_type").IsRequired();
        builder.Property(e => e.PlayerId).HasColumnName("player_id").IsRequired();
        builder.Property(e => e.AddonLevel).HasColumnName("addon_level").IsRequired();
    }
}