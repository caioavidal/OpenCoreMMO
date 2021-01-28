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
    public class GuildMembershipModelConfiguration : IEntityTypeConfiguration<GuildMembershipModel>
    {
        public void Configure(EntityTypeBuilder<GuildMembershipModel> builder)
        {
            builder.ToTable("guild_membership");

            builder.HasKey(e => new { e.PlayerId} );

            builder.Property(e => e.PlayerId).HasColumnName("player_id");
            builder.Property(e => e.GuildId).HasColumnName("guild_id");
            builder.Property(e => e.RankId).HasColumnName("rank_id");
            builder.Property(e => e.Nick).HasColumnName("nick");
        }
    }
}
