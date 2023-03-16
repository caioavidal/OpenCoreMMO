using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;
using NeoServer.Data.Seeds;

namespace NeoServer.Data.Configurations;

public class PlayerOutfitAddonModelConfiguration : IEntityTypeConfiguration<PlayerOutfitAddonModel>
{
    public void Configure(EntityTypeBuilder<PlayerOutfitAddonModel> builder)
    {
        builder.ToTable("player_outfit_addon");

        builder.HasKey(e => new { e.PlayerId, e.LookType, e.AddonLevel });

        builder.Property(e => e.LookType).HasColumnName("look_type").IsRequired();
        builder.Property(e => e.PlayerId).HasColumnName("player_id").IsRequired();
        builder.Property(e => e.AddonLevel).HasColumnName("addon_level").IsRequired();
    }
}