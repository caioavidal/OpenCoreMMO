using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Configurations
{
    public class ForSQLitePlayerItemModelConfiguration : IEntityTypeConfiguration<PlayerItemModel>
    {
        public void Configure(EntityTypeBuilder<PlayerItemModel> entity)
        {
            entity.ToTable("player_items");

            //entity.HasIndex(e => e.PlayerId)
            //    .HasDatabaseName("player_id");

            //entity.HasIndex(e => e.ServerId)
            //    .HasDatabaseName("sid");

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

            entity.Property(e => e.PlayerId)
                .HasColumnName("player_id")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.ServerId)
                .HasColumnName("sid")
                .IsRequired()
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.HasOne(d => d.Player)
                .WithMany(p => p.PlayerItems)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("player_items_ibfk_1");

            Seed(entity);
        }

        public void Seed(EntityTypeBuilder<PlayerItemModel> builder)
        {
            builder.HasData(
                new PlayerItemModel
                {
                    Id = -10,
                    PlayerId = 1,
                    ParentId = 0,
                    ServerId = 1988,
                    Amount = 1,
                }
            );
        }
    }
}
