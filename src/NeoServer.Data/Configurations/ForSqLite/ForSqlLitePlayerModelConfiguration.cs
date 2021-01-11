using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Seeds;
using NeoServer.Server.Model.Players;

namespace NeoServer.Data.Configurations
{
    public class ForSQLitePlayerModelConfiguration : IEntityTypeConfiguration<PlayerModel>
    {
        public void Configure(EntityTypeBuilder<PlayerModel> entity)
        {
            entity.ToTable("players");

            entity.HasKey(e => e.PlayerId);

            //entity.HasIndex(e => e.AccountId)
            //    .HasDatabaseName("account_id");

            //entity.HasIndex(e => e.Name)
            //    .HasDatabaseName("name")
            //    .IsUnique();

            //entity.HasIndex(e => e.Vocation)
            //    .HasDatabaseName("vocation");

            entity.Property(e => e.PlayerId)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.PlayerType)
              .HasColumnName("player_type");
            entity.Property(e => e.AccountId)
                .HasColumnName("account_id")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.Capacity)
                .HasColumnName("cap")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.Health)
                .HasColumnName("health")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("150");

            entity.Property(e => e.MaxHealth)
                .HasColumnName("healthmax")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("150");

            entity.Property(e => e.Level)
                .HasColumnName("level")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("1");

            entity.Property(e => e.LookAddons)
                .HasColumnName("lookaddons")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.LookBody)
                .HasColumnName("lookbody")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.LookFeet)
                .HasColumnName("lookfeet")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.LookHead)
                .HasColumnName("lookhead")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.LookLegs)
                .HasColumnName("looklegs")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.LookType)
                .HasColumnName("looktype")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("136");

            entity.Property(e => e.Mana)
                .HasColumnName("mana")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.MaxMana)
                .HasColumnName("manamax")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("varchar(255)");

            entity.Property(e => e.OfflineTrainingSkill)
                .HasColumnName("offlinetraining_skill")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("-1");

            entity.Property(e => e.OfflineTrainingTime)
                .HasColumnName("offlinetraining_time")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("4200");

            entity.Property(e => e.PosX)
                .HasColumnName("posx")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.PosY)
                .HasColumnName("posy")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.PosZ)
                .HasColumnName("posz")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.Gender)
                .HasColumnName("sex")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.SkillAxe)
                .HasColumnName("skill_axe")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("10");

            entity.Property(e => e.SkillAxeTries)
                .HasColumnName("skill_axe_tries")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.SkillClub)
                .HasColumnName("skill_club")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("10");

            entity.Property(e => e.SkillClubTries)
                .HasColumnName("skill_club_tries")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.SkillDist)
                .HasColumnName("skill_dist")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("10");

            entity.Property(e => e.SkillDistTries)
                .HasColumnName("skill_dist_tries")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.SkillFishing)
                .HasColumnName("skill_fishing")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("10");

            entity.Property(e => e.SkillFishingTries)
                .HasColumnName("skill_fishing_tries")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.SkillFist)
                .HasColumnName("skill_fist")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("10");

            entity.Property(e => e.SkillFistTries)
                .HasColumnName("skill_fist_tries")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.SkillShielding)
                .HasColumnName("skill_shielding")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("10");

            entity.Property(e => e.SkillShieldingTries)
                .HasColumnName("skill_shielding_tries")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.SkillSword)
                .HasColumnName("skill_sword")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("10");

            entity.Property(e => e.SkillSwordTries)
                .HasColumnName("skill_sword_tries")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.Vocation)
                .HasColumnName("vocation")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.HasOne(d => d.Account)
                .WithMany(p => p.Players)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("players_ibfk_1");

            PlayerModelSeed.Seed(entity);
        }


   
    }
}
