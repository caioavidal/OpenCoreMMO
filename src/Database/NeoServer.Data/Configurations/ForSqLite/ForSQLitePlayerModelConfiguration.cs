using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Entities;
using NeoServer.Data.Seeds;

namespace NeoServer.Data.Configurations.ForSqLite
{
    public class ForSQLitePlayerModelConfiguration : IEntityTypeConfiguration<PlayerEntity>
    {
        public void Configure(EntityTypeBuilder<PlayerEntity> entity)
        {
            entity.ToTable("players");

            entity.HasKey(e => e.PlayerId);

            entity.HasIndex(e => e.AccountId)
                .HasDatabaseName("account_id");

            entity.HasIndex(e => e.Name)
                .HasDatabaseName("name")
                .IsUnique();

            entity.HasIndex(e => e.Vocation)
                .HasDatabaseName("vocation");

            entity.Property(e => e.PlayerId)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.PlayerType)
                .HasColumnName("player_type");

            ConfigureProperty(entity, e => e.AccountId, "account_id", "int(11)", "0");
            ConfigureProperty(entity, e => e.TownId, "town", "int(11)", "1");
            ConfigureProperty(entity, e => e.Capacity, "cap", "int(11)", "0");
            ConfigureProperty(entity, e => e.Health, "health", "int(11)", "150");
            ConfigureProperty(entity, e => e.MaxHealth, "healthmax", "int(11)", "150");
            ConfigureProperty(entity, e => e.Level, "level", "int(11)", "1");
            ConfigureProperty(entity, e => e.LookAddons, "lookaddons", "int(11)", "0");
            ConfigureProperty(entity, e => e.LookBody, "lookbody", "int(11)", "0");
            ConfigureProperty(entity, e => e.LookFeet, "lookfeet", "int(11)", "0");
            ConfigureProperty(entity, e => e.LookHead, "lookhead", "int(11)", "0");
            ConfigureProperty(entity, e => e.LookLegs, "looklegs", "int(11)", "0");
            ConfigureProperty(entity, e => e.LookType, "looktype", "int(11)", "136");
            ConfigureProperty(entity, e => e.Mana, "mana", "int(11)", "0");
            ConfigureProperty(entity, e => e.MaxMana, "manamax", "int(11)", "0");
            ConfigureProperty(entity, e => e.Name, "name", "varchar(255)", null);
            ConfigureProperty(entity, e => e.OfflineTrainingSkill, "offlinetraining_skill", "int(11)", "-1");
            ConfigureProperty(entity, e => e.OfflineTrainingTime, "offlinetraining_time", null, "4200");
            ConfigureProperty(entity, e => e.PosX, "posx", "int(11)", "0");
            ConfigureProperty(entity, e => e.PosY, "posy", "int(11)", "0");
            ConfigureProperty(entity, e => e.PosZ, "posz", "int(11)", "0");
            ConfigureProperty(entity, e => e.Gender, "sex", "int(11)", "0");
            ConfigureProperty(entity, e => e.SkillAxe, "skill_axe", null, "10");
            ConfigureProperty(entity, e => e.SkillAxeTries, "skill_axe_tries", null, "0");
            ConfigureProperty(entity, e => e.SkillClub, "skill_club", null, "10");
            ConfigureProperty(entity, e => e.SkillClubTries, "skill_club_tries", null, "0");
            ConfigureProperty(entity, e => e.SkillDist, "skill_dist", null, "10");
            ConfigureProperty(entity, e => e.SkillDistTries, "skill_dist_tries", null, "0");
            ConfigureProperty(entity, e => e.SkillFishing, "skill_fishing", null, "10");
            ConfigureProperty(entity, e => e.SkillFishingTries, "skill_fishing_tries", null, "0");
            ConfigureProperty(entity, e => e.SkillFist, "skill_fist", null, "10");
            ConfigureProperty(entity, e => e.SkillFistTries, "skill_fist_tries", null, "0");
            ConfigureProperty(entity, e => e.SkillShielding, "skill_shielding", null, "10");
            ConfigureProperty(entity, e => e.SkillShieldingTries, "skill_shielding_tries", null, "0");
            ConfigureProperty(entity, e => e.Online, "online", "boolean", "0");
            ConfigureProperty(entity, e => e.SkillSword, "skill_sword", null, "10");
            ConfigureProperty(entity, e => e.SkillSwordTries, "skill_sword_tries", null, "0");
            ConfigureProperty(entity, e => e.Vocation, "vocation", "int(11)", "0");
            ConfigureProperty(entity, e => e.RemainingRecoverySeconds, "remaining_recovery_seconds", "int(11)", "0");

            entity.HasOne(d => d.Account)
                .WithMany(p => p.Players)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("players_ibfk_1");

            entity.HasOne(d => d.World)
                .WithMany()
                .HasForeignKey(d => d.WorldId);

            entity.HasOne(x => x.GuildMember).WithOne(x => x.Player);

            PlayerModelSeed.Seed(entity);
        }

        private void ConfigureProperty<TProperty>(
            EntityTypeBuilder<PlayerEntity> entity,
            Expression<Func<PlayerEntity, TProperty>> property,
            string columnName,
            string columnType,
            string defaultValueSql = null)
        {
            entity.Property(property)
                .HasColumnName(columnName)
                .HasColumnType(columnType)
                .HasAnnotation("Sqlite:Autoincrement", false)
                .HasDefaultValueSql(defaultValueSql);
        }
    }
}
