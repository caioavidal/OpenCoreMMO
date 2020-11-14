using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Combat;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Creatures.Players;
using NeoServer.Game.Enums.Item;
using System.Linq;

namespace NeoServer.Game.Creatures.Model.Bases
{

    public abstract class CombatActor : WalkableCreature, ICombatActor
    {
        #region Events
        public event Heal OnHeal;
        public event StopAttack OnStoppedAttack;
        public event BlockAttack OnBlockedAttack;
        public event Attack OnAttackEnemy;
        public event Damage OnDamaged;
        public event Die OnKilled;
        public event OnAttackTargetChange OnTargetChanged;
        public event ChangeVisibility OnChangedVisibility;
        #endregion

        #region Properties
        public bool IsDead => HealthPoints <= 0;
        public decimal BaseAttackSpeed => 2000M;
        public decimal BaseDefenseSpeed { get; }
        public bool InFight => Conditions.Any(x => x.Key == ConditionType.InFight);
        public abstract ushort AttackPower { get; }
        public abstract ushort ArmorRating { get; }
        public abstract ushort DefensePower { get; }
        public uint AutoAttackTargetId { get; private set; }
        public bool Attacking => AutoAttackTargetId > 0;
        public abstract byte AutoAttackRange { get; }
        public abstract ushort MinimumAttackPower { get; }
        public abstract bool UsingDistanceWeapon { get; }
        public uint AttackEvent { get; set; }
        #endregion

        private byte blockCount = 0;
        private const byte BLOCK_LIMIT = 2;

        protected CombatActor(ICreatureType type, PathFinder pathFinder, IOutfit outfit = null, uint healthPoints = 0) : base(type, pathFinder, outfit, healthPoints)
        {
        }
        public abstract int ShieldDefend(int attack);
        public abstract int ArmorDefend(int attack);
        private bool canBlock(DamageType damage)
        {
            if (damage != DamageType.Melee) return false;
            var hasCoolDownExpired = Cooldowns.Expired(CooldownType.Block);

            if (!hasCoolDownExpired && blockCount >= BLOCK_LIMIT)
            {
                return false;
            }
            return true;
        }
        private void block()
        {
            if (Cooldowns.Expired(CooldownType.Block))
            {
                Cooldowns.Start(CooldownType.Block, 2000);
                blockCount = 0;
            }

            blockCount++;
        }
        public void ResetHealthPoints() => HealthPoints = MaxHealthpoints;

        public ushort ReduceDamage(CombatDamage attack)
        {
            int damage;

            if (canBlock(attack.Type))
            {
                damage = ShieldDefend(attack.Damage);

                if (damage <= 0)
                {
                    damage = 0;

                    block();
                    OnBlockedAttack?.Invoke(this, BlockType.Shield);
                    return (ushort)damage;
                }
            }

            damage = ArmorDefend(attack.Damage);

            if (damage <= 0)
            {
                damage = 0;
                OnBlockedAttack?.Invoke(this, BlockType.Armor);
            }

            return (ushort)damage;
        }

        public void StopAttack()
        {
            AutoAttackTargetId = 0;
            OnStoppedAttack?.Invoke(this);
        }
        public bool WasDamagedOnLastAttack = true;

        //public virtual bool MagicalAttack(ICombatAttack enemy)
        //{

        //}

        public abstract bool OnAttack(ICombatActor enemy, out CombatAttackType combat);

        public bool Attack(ICombatActor enemy)
        {
            if (enemy.IsDead)
            {
                StopAttack();
                return false;
            }

            if (!Cooldowns.Expired(CooldownType.Combat)) return false;

            SetAttackTarget(enemy.CreatureId);

            if (!OnAttack(enemy, out var combat)) return false;

            OnAttackEnemy?.Invoke(this, enemy, combat);

            Cooldowns.Start(CooldownType.Combat, (int)BaseAttackSpeed);

            return true;
        }
        public virtual void SetAttackTarget(uint targetId)
        {
            if (targetId == AutoAttackTargetId) return;

            var oldAttackTarget = AutoAttackTargetId;
            AutoAttackTargetId = targetId;

            if (targetId == 0)
            {
                StopAttack();
                StopFollowing();
            }
            OnTargetChanged?.Invoke(this, oldAttackTarget, targetId);
        }

        private void ReduceHealth(ushort damage)
        {
            HealthPoints = damage > HealthPoints ? 0 : HealthPoints - damage;

            if (IsDead) OnKilled?.Invoke(this);
        }
        public void Heal(ushort increasing)
        {
            HealthPoints = HealthPoints + increasing >= MaxHealthpoints ? MaxHealthpoints : HealthPoints + increasing;
            OnHeal?.Invoke(this, increasing);
        }

        public void TurnInvisible()
        {
            IsInvisible = true;
            OnChangedVisibility?.Invoke(this);
        }
        public void TurnVisible()
        {
            IsInvisible = false;
            OnChangedVisibility?.Invoke(this);
        }
        public void StartSpellCooldown(ISpell spell) => Cooldowns.Start(spell.Name, (int)spell.Cooldown);
        public bool SpellCooldownHasExpired(ISpell spell) => Cooldowns.Expired(spell.Name);
        public bool CooldownHasExpired(CooldownType type) => Cooldowns.Expired(type);

        public void ReceiveAttack(ICombatActor enemy, CombatDamage damage)
        {
            var damageValue = damage.Damage;
            if (!damage.IsElementalDamage)
            {
                damageValue = ReduceDamage(damage);
            }
            damage.ReduceDamage(damageValue);

            if (damage.Damage <= 0)
            {
                WasDamagedOnLastAttack = false;
                return;
            }

            if (IsDead)
            {
                return;
            }

            if (damage.Type != DamageType.ManaDrain)
            {
                ReduceHealth(damage.Damage);
            }

            OnDamaged?.Invoke(enemy, this, damage);
            WasDamagedOnLastAttack = true;
            return;
        }

        public virtual ushort CalculateAttackPower(float attackRate, ushort attack) { return 0; }
    }
}
