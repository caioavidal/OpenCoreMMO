using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;

namespace NeoServer.Data.Configurations
{
    public class PlayerDepotItemModelConfiguration : IEntityTypeConfiguration<PlayerDepotItemModel>
    {
        public void Configure(EntityTypeBuilder<PlayerDepotItemModel> entity)
        {
            entity.HasKey(e => e.Id )
                    .HasName("player_depot_items_id");

            entity.ToTable("player_depot_items");

            entity.Property(e => e.PlayerId)
                .HasColumnName("player_id")
                .HasColumnType("int(11)");

            entity.Property(e => e.ServerId)
                .HasColumnName("sid")
                .HasColumnType("int(11)");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Amount)
                .HasColumnName("count")
                .HasColumnType("smallint(5)")
                .HasDefaultValueSql("0");

            entity.Property(e => e.ParentId)
                .HasColumnName("pid")
                .HasColumnType("int(11)")
                .HasDefaultValueSql("0");

            entity.HasOne(d => d.Player)
                .WithMany(p => p.PlayerDepotItems)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("player_depot_items_ibfk_1");

            Seed(entity);
        }

        public void Seed(EntityTypeBuilder<PlayerDepotItemModel> builder)
        {
            //builder.HasData();
        }
    }
}
