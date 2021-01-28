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
    public class GuildModelConfiguration : IEntityTypeConfiguration<GuildModel>
    {
        public void Configure(EntityTypeBuilder<GuildModel> builder)
        {
            builder.ToTable("guilds");

            builder.HasKey(e => new { e.Id} );

            builder.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            builder.Property(e => e.Name).HasColumnName("name");
            builder.Property(e => e.OwnerId).HasColumnName("ownerid");
            builder.Property(e => e.CreationDate).HasColumnName("creation_date");
            builder.Property(e => e.Modt).HasColumnName("modt");
        }
    }
}
