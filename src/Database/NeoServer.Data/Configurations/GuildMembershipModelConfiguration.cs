using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Entities;

namespace NeoServer.Data.Configurations;

public class GuildMembershipModelConfiguration : IEntityTypeConfiguration<GuildMembershipEntity>
{
    public void Configure(EntityTypeBuilder<GuildMembershipEntity> builder)
    {
        builder.ToTable("guild_membership");

        builder.HasKey(e => new { e.PlayerId, e.GuildId });

        builder.Property(e => e.PlayerId).HasColumnName("player_id");
        builder.Property(e => e.GuildId).HasColumnName("guild_id");
        builder.Property(e => e.RankId).HasColumnName("rank_id");
        builder.Property(e => e.Nick).HasColumnName("nick");

        builder.HasOne(x => x.Guild).WithMany(x => x.Members).HasForeignKey("GuildId");
    }
}