using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
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
        public event OnPropagateAttack OnPropagateAttack;
        #endregion

        #region Properties
        public bool IsDead => HealthPoints <= 0;
        public decimal BaseAttackSpeed => 2000M;
        public decimal BaseDefenseSpeed { get; }
        public bool InFight => Conditions.Any(x => x.Key == ConditionType.InFight);
        public abstract ushort ArmorRating { get; }
        public uint AutoAttackTargetId => AutoAttackTarget?.CreatureId ?? default;
        public ICreature AutoAttackTarget { get; private set; }
        public bool Attacking => AutoAttackTargetId > 0;
        public abstract ushort MinimumAttackPower { get; }
        public abstract bool UsingDistanceWeapon { get; }
        public uint AttackEvent { get; set; }
        public virtual bool CanBeAttacked => true;
        #endregion

        private byte blockCount = 0;
        private const byte BLOCK_LIMIT = 2;

        protected CombatActor(ICreatureType type, IPathAccess pathAccess, IOutfit outfit = null, uint healthPoints = 0) : base(type, pathAccess, outfit, healthPoints)
        {
        }
        public abstract int ShieldDefend(int attack);
        public abstract int ArmorDefend(int attack);
        public virtual bool CanBlock(DamageType damage)
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
        public void ResetHealthPoints() => Heal((ushort)MaxHealthPoints);

        public CombatDamage ReduceDamage(CombatDamage attack)
        {
            int damage = attack.Damage;

            if (CanBlock(attack.Type))
            {
                damage = ShieldDefend(attack.Damage);

                if (damage <= 0)
                {
                    damage = 0;

                    block();
                    OnBlockedAttack?.Invoke(this, BlockType.Shield);
                    attack.SetNewDamage((ushort)damage);
                    return attack;
                }
            }

            if (!attack.IsElementalDamage) damage = ArmorDefend(attack.Damage);

            if (damage <= 0)
            {
                damage = 0;
                OnBlockedAttack?.Invoke(this, BlockType.Armor);

            }
            attack.SetNewDamage((ushort)damage);

            return OnImmunityDefense(attack);

            //return attack;
        }

        public void StopAttack()
        {
            if (!Attacking) return;

            StopFollowing();
            AutoAttackTarget = null;
            OnStoppedAttack?.Invoke(this);
        }
        private bool WasDamagedOnLastAttack = true;

        public abstract bool OnAttack(ICombatActor enemy, out CombatAttackType combat);

        public bool Attack(ITile tile, IUseableAttackOnTile item)
        {
            if (!CanSee(tile.Location)) return false;

            if (!item.Use(this, tile, out var combat)) return false;
            var creature = tile is IDynamicTile t ? tile.TopCreatureOnStack : null;
            OnAttackEnemy?.Invoke(this, creature, combat);

            return true;
        }
        public bool Attack(ICreature creature, IUseableAttackOnCreature item)
        {
            if (creature is not ICombatActor enemy || enemy.IsDead || IsDead || !CanSee(creature.Location)) return false;

            if (!item.Use(this, creature, out var combat)) return false;
            OnAttackEnemy?.Invoke(this, enemy, combat);

            return true;
        }
        public bool Attack(ICreature creature)
        {
            if (creature is not ICombatActor enemy || enemy.IsDead || IsDead || !CanSee(creature.Location))
            {
                StopAttack();
                return false;
            }

            if (!Cooldowns.Expired(CooldownType.Combat)) return false;

            SetAttackTarget(enemy);

            if (!OnAttack(enemy, out var combat)) return false;

            OnAttackEnemy?.Invoke(this, enemy, combat);

            Cooldowns.Start(CooldownType.Combat, (int)BaseAttackSpeed);

            return true;
        }

        public virtual void SetAttackTarget(ICreature target)
        {
            if (target is not ICombatActor) return;
            if (target?.CreatureId == AutoAttackTargetId) return;

            var oldAttackTarget = AutoAttackTargetId;
            AutoAttackTarget = target;

            if (target?.CreatureId == 0)
            {
                StopAttack();
                StopFollowing();
            }

            OnTargetChanged?.Invoke(this, oldAttackTarget, target?.CreatureId ?? default);
        }

        protected void ReduceHealth(CombatDamage damage)
        {
            HealthPoints = damage.Damage > HealthPoints ? 0 : HealthPoints - damage.Damage;
        }
        public virtual void OnDeath()
        {
            StopAttack();
            StopFollowing();
            StopWalking();
            Conditions.Clear();
            OnKilled?.Invoke(this);
        }
        public void Heal(ushort increasing)
        {
            if (increasing <= 0) return;

            if (HealthPoints == MaxHealthPoints) return;

            HealthPoints = HealthPoints + increasing >= MaxHealthPoints ? MaxHealthPoints : HealthPoints + increasing;
            OnHeal?.Invoke(this, increasing);
        }

        public virtual void TurnInvisible()
        {
            IsInvisible = true;
            OnChangedVisibility?.Invoke(this);
        }
        public virtual void TurnVisible()
        {
            IsInvisible = false;
            OnChangedVisibility?.Invoke(this);
        }
        public void StartSpellCooldown(ISpell spell) => Cooldowns.Start(spell.Name, (int)spell.Cooldown);
        public bool SpellCooldownHasExpired(ISpell spell) => Cooldowns.Expired(spell.Name);
        public bool CooldownHasExpired(CooldownType type) => Cooldowns.Expired(type);

        public abstract void OnDamage(IThing enemy, CombatDamage damage);

        private void OnDamage(IThing enemy, ICombatActor actor, CombatDamage damage)
        {
            OnDamage(enemy, damage);
            OnDamaged?.Invoke(enemy, this, damage);
            if (IsDead) OnDeath();
        }
     
        public virtual bool ReceiveAttack(IThing enemy, CombatDamage damage)
        {
            if (!CanBeAttacked) return false;
            if (IsDead) return false;

            if(this is ICreature c) SetAsEnemy(c);

            damage = ReduceDamage(damage);
            if (damage.Damage <= 0)
            {
                WasDamagedOnLastAttack = false;
                return false;
            }

            OnDamage(enemy, this, damage);

            WasDamagedOnLastAttack = true;
            return true;
        }
        public void PropagateAttack(Coordinate[] area, CombatDamage damage)
        {
            if (IsDead) return;
            if (damage.Damage <= 0) return;

            OnPropagateAttack?.Invoke(this, damage, area);
        }

        public abstract CombatDamage OnImmunityDefense(CombatDamage damage);

        public abstract void SetAsEnemy(ICreature actor);
    }
}
