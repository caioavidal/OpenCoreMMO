using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;
using NeoServer.Data.Seeds;

namespace NeoServer.Data.Configurations.ForSqLite;

public class ForSQLiteWorldModelConfiguration : IEntityTypeConfiguration<WorldModel>
{
    public void Configure(EntityTypeBuilder<WorldModel> builder)
    {
        builder.ToTable("worlds");

        builder.HasKey(e => new { e.Id });

        builder.Property(e => e.Id).HasAnnotation("Sqlite:Autoincrement", false).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name").IsRequired();
        builder.Property(e => e.Ip).HasColumnName("ip").IsRequired();
        
        WorldModelSeed.Seed(builder);
    }
}