using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;

namespace NeoServer.Data.Configurations
{
    public class ForSQLiteGuildModelConfiguration : IEntityTypeConfiguration<GuildModel>
    {
        public void Configure(EntityTypeBuilder<GuildModel> builder)
        {
            builder.ToTable("guilds");

            builder.HasKey(e => new { e.Id} );

            builder.Property(e => e.Id).HasAnnotation("Sqlite:Autoincrement", false).HasColumnName("id");
            builder.Property(e => e.Name).HasColumnName("name");
            builder.Property(e => e.OwnerId).HasColumnName("ownerid");
            builder.Property(e => e.CreationDate).HasColumnName("creation_date");
            builder.Property(e => e.Modt).HasColumnName("modt");

            builder.HasMany(x => x.Members).WithOne().HasForeignKey("GuildId");

        }
    }
}
