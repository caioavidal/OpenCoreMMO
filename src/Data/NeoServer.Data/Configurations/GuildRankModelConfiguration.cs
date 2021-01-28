using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Configurations
{
    public class GuildRankModelConfiguration : IEntityTypeConfiguration<GuildRankModel>
    {
        public void Configure(EntityTypeBuilder<GuildRankModel> builder)
        {
            builder.ToTable("guild_ranks");

            builder.HasKey(e => new { e.Id} );

            builder.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            builder.Property(e => e.GuildId).HasColumnName("guild_id");
            builder.Property(e => e.Name).HasColumnName("name");
            builder.Property(e => e.Level).HasColumnName("level");
        }
    }
}
