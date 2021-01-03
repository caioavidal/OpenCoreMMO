using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;

namespace NeoServer.Data.Configurations
{
    public class AccountModelConfiguration : IEntityTypeConfiguration<AccountModel>
    {
        public void Configure(EntityTypeBuilder<AccountModel> builder)
        {
            builder.ToTable("accounts");

            builder.HasKey(e => e.AccountId);

            builder.HasIndex(e => e.Name)
                 .HasDatabaseName("name")
                 .IsUnique();

            builder.Property(e => e.AccountId)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
                //.HasColumnType("int(11)");

            builder.Property(e => e._creation)
                .HasColumnName("creation")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false)
                .HasDefaultValueSql("0");

            builder.Property(e => e.Email)
                .IsRequired()
                .HasColumnName("email")
                .HasColumnType("varchar(255)");

            builder.Property(e => e._lastday)
                .HasColumnName("lastday")
                .HasColumnType("int(10) unsigned")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("varchar(32)");

            builder.Property(e => e.Password)
                .IsRequired()
                .HasColumnName("password")
                .HasColumnType("char(255)");

            builder.Property(e => e.PremiumTime)
                .HasColumnName("premdays")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            builder.Property(e => e.Secret)
                .HasColumnName("secret")
                .HasColumnType("char(16)");

            builder.Property(e => e.Type)
                .HasColumnName("type")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("1");

            builder.Ignore(i => i.Creation);

            builder.Ignore(i => i.LastDay);

            Seed(builder);
        }

        public void Seed(EntityTypeBuilder<AccountModel> builder)
        {
            builder.HasData
                (
                new AccountModel
                {
                    AccountId = 1,
                    Name = "1",
                    Email = "god@gmail.com",
                    Password = "1",
                    PremiumTime = 1
                }
              );
        }
    }
}
