using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Creatures.Model.Bases
{
    public abstract class CombatActor : WalkableCreature, ICombatActor
    {
        private const byte BLOCK_LIMIT = 2;

        private byte blockCount;
        private bool WasDamagedOnLastAttack = true;

        protected CombatActor(ICreatureType type, IMapTool mapTool, IOutfit outfit = null, uint healthPoints = 0) : base(type,mapTool, outfit,
            healthPoints)
        {
        }

        public abstract int ShieldDefend(int attack);
        public abstract int ArmorDefend(int attack);

        public void AddCondition(ICondition condition)
        {
            var result = Conditions.TryAdd(condition.Type, condition);
            condition.Start(this);
            if (result == false) return;

            OnAddedCondition?.Invoke(this, condition);
        }

        public void RemoveCondition(ICondition condition)
        {
            Conditions.Remove(condition.Type);
            OnRemovedCondition?.Invoke(this, condition);
        }

        public void RemoveCondition(ConditionType type)
        {
            if (Conditions.Remove(type, out var condition) is false) return;
            OnRemovedCondition?.Invoke(this, condition);
        }

        public bool HasCondition(ConditionType type, out ICondition condition)
        {
            return Conditions.TryGetValue(type, out condition);
        }

        public bool HasCondition(ConditionType type)
        {
            return Conditions.ContainsKey(type);
        }

        public void ResetHealthPoints()
        {
            Heal((ushort)MaxHealthPoints, this);
        }

        public virtual void GainExperience(uint exp)
        {
            OnGainedExperience?.Invoke(this, exp);
        }

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

            attack = OnImmunityDefense(attack);

            if (attack.Damage <= 0) OnBlockedAttack?.Invoke(this, BlockType.Armor);

            return attack;
        }

        public void StopAttack()
        {
            if (!Attacking) return;

            StopFollowing();
            AutoAttackTarget = null;
            OnStoppedAttack?.Invoke(this);
        }

        public bool Attack(ICreature creature, IUsableAttackOnCreature item)
        {
            if (creature is not ICombatActor enemy || enemy.IsDead || IsDead || !CanSee(creature.Location) ||
                creature.Equals(this)) return false;
            
            if (item.NeedTarget && MapTool.SightClearChecker?.Invoke(Location, enemy.Location) == false) return false;

            if (!item.Use(this, creature, out var combat)) return false;
            OnAttackEnemy?.Invoke(this, enemy, combat);

            return true;
        }

        public bool Attack(ICreature creature)
        {
            if (creature is not ICombatActor enemy || enemy.IsDead || IsDead || !CanSee(creature.Location) ||
                creature.Equals(this))
            {
                StopAttack();
                return false;
            }

            if (!Cooldowns.Expired(CooldownType.Combat)) return false;

            SetAttackTarget(enemy);

            if (MapTool.SightClearChecker?.Invoke(Location, enemy.Location) == false) return false;

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

        public void Heal(ushort increasing, ICombatActor healedBy)
        {
            if (increasing <= 0) return;

            if (HealthPoints == MaxHealthPoints) return;

            HealthPoints = HealthPoints + increasing >= MaxHealthPoints ? MaxHealthPoints : HealthPoints + increasing;
            OnHeal?.Invoke(this, healedBy, increasing);
        }

        public virtual void TurnInvisible()
        {
            IsInvisible = true;
            OnChangedVisibility?.Invoke(this);
        }

        public override Direction GetNextStep()
        {
            if (IsDead) return Direction.None;
            return base.GetNextStep();
        }

        public virtual void TurnVisible()
        {
            IsInvisible = false;
            OnChangedVisibility?.Invoke(this);
        }

        public void StartSpellCooldown(ISpell spell)
        {
            Cooldowns.Start(spell.Name, (int)spell.Cooldown);
        }

        public bool SpellCooldownHasExpired(ISpell spell)
        {
            return Cooldowns.Expired(spell.Name);
        }

        public bool CooldownHasExpired(CooldownType type)
        {
            return Cooldowns.Expired(type);
        }

        public virtual bool ReceiveAttack(IThing enemy, CombatDamage damage)
        {

            if (enemy?.Equals(this) ?? false) return false;
            if (!CanBeAttacked) return false;
            if (IsDead) return false;

            OnAttacked?.Invoke(enemy, this, ref damage);

            if (enemy is ICreature c) SetAsEnemy(c);

            damage = ReduceDamage(damage);
            if (damage.Damage <= 0)
            {
                WasDamagedOnLastAttack = false;
                return true;
            }

            OnDamage(enemy, this, damage);
            
            WasDamagedOnLastAttack = true;
            return true;
        }

        public void PropagateAttack(AffectedLocation[] area, CombatDamage damage)
        {
            if (IsDead) return;
            if (damage.Damage <= 0) return;

            OnPropagateAttack?.Invoke(this, damage, area);
        }

        public abstract void SetAsEnemy(ICreature actor);
        public abstract bool HasImmunity(Immunity immunity);

        public virtual bool CanBlock(DamageType damage)
        {
            if (damage != DamageType.Melee) return false;
            var hasCoolDownExpired = Cooldowns.Expired(CooldownType.Block);

            if (!hasCoolDownExpired && blockCount >= BLOCK_LIMIT) return false;
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

        public abstract bool OnAttack(ICombatActor enemy, out CombatAttackType combat);

        public bool Attack(ITile tile, IUsableAttackOnTile item)
        {
            if (!CanSee(tile.Location)) return false;
            
            if (MapTool.SightClearChecker?.Invoke(Location, tile.Location) == false) return false;


            if (!item.Use(this, tile, out var combat)) return false;
            
            var creature = tile is IDynamicTile t ? tile.TopCreatureOnStack : null;
            OnAttackEnemy?.Invoke(this, creature, combat);

            return true;
        }

        protected void ReduceHealth(CombatDamage damage)
        {
            HealthPoints = damage.Damage > HealthPoints ? 0 : HealthPoints - damage.Damage;
        }

        public abstract ILoot DropLoot();

        public virtual void OnDeath(IThing by)
        {
            StopAttack();
            StopFollowing();
            StopWalking();
            Conditions.Clear();
            var loot = DropLoot();
            OnKilled?.Invoke(this, by, loot);
        }

        public abstract void OnDamage(IThing enemy, CombatDamage damage);

        private void OnDamage(IThing enemy, ICombatActor actor, CombatDamage damage)
        {
            OnDamage(enemy, damage);
            OnInjured?.Invoke(enemy, this, damage);
            if (IsDead) OnDeath(enemy);
        }

        public abstract CombatDamage OnImmunityDefense(CombatDamage damage);

        #region Events

        public event Heal OnHeal;
        public event StopAttack OnStoppedAttack;
        public event BlockAttack OnBlockedAttack;
        public event Attack OnAttackEnemy;
        public event Damage OnInjured;
        public event Die OnKilled;
        public event AttackTargetChange OnTargetChanged;
        public event ChangeVisibility OnChangedVisibility;
        public event PropagateAttack OnPropagateAttack;
        public event GainExperience OnGainedExperience;
        public event AddCondition OnAddedCondition;
        public event RemoveCondition OnRemovedCondition;
        public event Attacked OnAttacked;
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
        public virtual bool CanBeAttacked => true; //todo: set as a flag

        public IDictionary<ConditionType, ICondition> Conditions { get; set; } =
            new Dictionary<ConditionType, ICondition>();

        #endregion
    }
}