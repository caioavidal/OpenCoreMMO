using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Entities;
using NeoServer.Data.Seeds;

namespace NeoServer.Data.Configurations;

public class PlayerItemModelConfiguration : IEntityTypeConfiguration<PlayerItemEntity>
{
    public void Configure(EntityTypeBuilder<PlayerItemEntity> entity)
    {
        entity.ToTable("player_items");

        entity.HasKey(x => x.Id);
        
        entity.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.PlayerId)
            .HasColumnName("player_id")
            .HasColumnType("int(11)");

        entity.Property(e => e.ServerId)
            .HasColumnName("sid")
            .HasColumnType("int(11)");

        entity.Property(e => e.Amount)
            .HasColumnName("count")
            .HasColumnType("smallint(5)")
            .HasDefaultValueSql("0");

        entity.Property(e => e.ParentId)
            .HasColumnName("pid")
            .HasColumnType("int(11)")
            .HasDefaultValueSql("0");

        entity.HasOne(d => d.Player)
            .WithMany(p => p.PlayerItems)
            .HasForeignKey(d => d.PlayerId)
            .HasConstraintName("player_items_ibfk_1");

        entity.Property(e => e.DecayTo)
            .HasColumnName("decayTo")
            .HasColumnType("int");

        entity.Property(e => e.DecayDuration)
            .HasColumnName("decayDuration")
            .HasColumnType("int");

        entity.Property(e => e.DecayElapsed)
            .HasColumnName("decayElapsed")
            .HasColumnType("int");

        entity.Property(e => e.Charges)
            .HasColumnName("charges")
            .HasColumnType("int");

        PlayerItemSeed.Seed(entity);
    }
}