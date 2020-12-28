using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;

namespace NeoServer.Data
{
    public class NeoContext : DbContext
    {
        public NeoContext(DbContextOptions<NeoContext> options)
            : base(options)
        { }

        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<PlayerModel> Player { get; set; }
        public DbSet<PlayerItemModel> PlayerItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AccountModel>(entity =>
            {
                entity.ToTable("accounts");

                entity.HasKey(e => e.AccountId);

               entity.HasIndex(e => e.Name)
                    .HasName("name")
                    .IsUnique();

                entity.Property(e => e.AccountId)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e._creation)
                    .HasColumnName("creation")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e._lastday)
                    .HasColumnName("lastday")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("char(255)");

                entity.Property(e => e.PremiumTime)
                    .HasColumnName("premdays")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Secret)
                    .HasColumnName("secret")
                    .HasColumnType("char(16)");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Ignore(i => i.Creation);

                entity.Ignore(i => i.LastDay);
            });

            modelBuilder.Entity<PlayerItemModel>(entity =>
            {
                entity.ToTable("player_items");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("player_id");

                entity.HasIndex(e => e.Sid)
                    .HasName("sid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Attributes)
                    .IsRequired()
                    .HasColumnName("attributes")
                    .HasColumnType("blob");

                entity.Property(e => e.Count)
                    .HasColumnName("count")
                    .HasColumnType("smallint(5)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Itemtype)
                    .HasColumnName("itemtype")
                    .HasColumnType("smallint(6)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Pid)
                    .HasColumnName("pid")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("player_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Sid)
                    .HasColumnName("sid")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerItems)
                    .HasForeignKey(d => d.PlayerId)
                    .HasConstraintName("player_items_ibfk_1");
            });

            modelBuilder.Entity<PlayerModel>(entity =>
            {
                entity.ToTable("players");

                entity.HasKey(e => e.PlayerId);

                entity.HasIndex(e => e.AccountId)
                    .HasName("account_id");

                entity.HasIndex(e => e.Name)
                    .HasName("name")
                    .IsUnique();

                entity.HasIndex(e => e.Vocation)
                    .HasName("vocation");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AccountId)
                    .HasColumnName("account_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Capacity)
                    .HasColumnName("cap")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Health)
                    .HasColumnName("health")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("150");

                entity.Property(e => e.MaxHealth)
                    .HasColumnName("healthmax")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("150");

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Lookaddons)
                    .HasColumnName("lookaddons")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Lookbody)
                    .HasColumnName("lookbody")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Lookfeet)
                    .HasColumnName("lookfeet")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Lookhead)
                    .HasColumnName("lookhead")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Looklegs)
                    .HasColumnName("looklegs")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Looktype)
                    .HasColumnName("looktype")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("136");

                entity.Property(e => e.Mana)
                    .HasColumnName("mana")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.MaxMana)
                    .HasColumnName("manamax")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.OfflineTrainingSkill)
                    .HasColumnName("offlinetraining_skill")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("-1");

                entity.Property(e => e.OfflineTrainingTime)
                    .HasColumnName("offlinetraining_time")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValueSql("4200");

                entity.Property(e => e.PosX)
                    .HasColumnName("posx")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.PosY)
                    .HasColumnName("posy")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.PosZ)
                    .HasColumnName("posz")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Gender)
                    .HasColumnName("sex")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SkillAxe)
                    .HasColumnName("skill_axe")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("10");

                entity.Property(e => e.SkillAxeTries)
                    .HasColumnName("skill_axe_tries")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SkillClub)
                    .HasColumnName("skill_club")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("10");

                entity.Property(e => e.SkillClubTries)
                    .HasColumnName("skill_club_tries")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SkillDist)
                    .HasColumnName("skill_dist")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("10");

                entity.Property(e => e.SkillDistTries)
                    .HasColumnName("skill_dist_tries")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SkillFishing)
                    .HasColumnName("skill_fishing")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("10");

                entity.Property(e => e.SkillFishingTries)
                    .HasColumnName("skill_fishing_tries")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SkillFist)
                    .HasColumnName("skill_fist")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("10");

                entity.Property(e => e.SkillFistTries)
                    .HasColumnName("skill_fist_tries")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SkillShielding)
                    .HasColumnName("skill_shielding")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("10");

                entity.Property(e => e.SkillShieldingTries)
                    .HasColumnName("skill_shielding_tries")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SkillSword)
                    .HasColumnName("skill_sword")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("10");

                entity.Property(e => e.SkillSwordTries)
                    .HasColumnName("skill_sword_tries")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Vocation)
                    .HasColumnName("vocation")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Players)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("players_ibfk_1");
            });
        }
    }
}
