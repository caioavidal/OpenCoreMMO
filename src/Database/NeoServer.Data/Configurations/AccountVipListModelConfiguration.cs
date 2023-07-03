using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Entities;

namespace NeoServer.Data.Configurations;

public class AccountVipListModelConfiguration : IEntityTypeConfiguration<AccountVipListEntity>
{
    public void Configure(EntityTypeBuilder<AccountVipListEntity> builder)
    {
        builder.ToTable("account_viplist");

        builder.HasKey(e => new { e.AccountId, e.PlayerId });

        builder.Property(e => e.AccountId).HasColumnName("account_id");
        builder.Property(e => e.PlayerId).HasColumnName("player_id");
        builder.Property(e => e.Description).HasColumnName("description");
    }
}