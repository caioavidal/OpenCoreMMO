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
    public class AccountVipListModelConfiguration : IEntityTypeConfiguration<AccountVipListModel>
    {
        public void Configure(EntityTypeBuilder<AccountVipListModel> builder)
        {
            builder.ToTable("account_viplist");

            builder.HasKey(e => new { e.AccountId, e.PlayerId} );

            builder.Property(e => e.AccountId).HasColumnName("account_id");
            builder.Property(e => e.PlayerId).HasColumnName("player_id");
            builder.Property(e => e.Description).HasColumnName("description");
        }
    }
}
