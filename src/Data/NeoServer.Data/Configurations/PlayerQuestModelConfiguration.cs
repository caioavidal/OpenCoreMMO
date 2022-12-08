using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;

namespace NeoServer.Data.Configurations;

public class PlayerQuestModelConfiguration : IEntityTypeConfiguration<PlayerQuestModel>
{
    public void Configure(EntityTypeBuilder<PlayerQuestModel> builder)
    {
        builder.ToTable("player_quests");

        builder.HasKey(e => new { e.ActionId, e.UniqueId });

        builder.Property(e => e.ActionId).HasColumnName("action_id").IsRequired();
        builder.Property(e => e.UniqueId).HasColumnName("unique_id").IsRequired();
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.Done).HasColumnName("done");
        builder.Property(e => e.PlayerId).HasColumnName("player_id").IsRequired();

        builder.HasOne(e => e.Player).WithMany().HasForeignKey(x => x.PlayerId);
    }
}