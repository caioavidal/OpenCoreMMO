using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;

namespace NeoServer.Data.Configurations.ForSqLite;

public class ForSQLitePlayerDepotItemModelConfiguration : IEntityTypeConfiguration<PlayerDepotItemModel>
{
    public void Configure(EntityTypeBuilder<PlayerDepotItemModel> entity)
    {
        entity.ToTable("player_depot_items");

        entity.Property(e => e.PlayerId)
            .HasColumnName("player_id")
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Amount)
            .HasColumnName("count")
            .IsRequired()
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("1");

        entity.Property(e => e.ParentId)
            .HasColumnName("pid")
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.Property(e => e.ServerId)
            .HasColumnName("sid")
            .IsRequired()
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.HasOne(d => d.Player)
            .WithMany(p => p.PlayerDepotItems)
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

        Seed(entity);
    }

    public void Seed(EntityTypeBuilder<PlayerDepotItemModel> builder)
    {
        //builder.HasData();
    }
}