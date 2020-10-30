using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Combat;
using NeoServer.Game.Enums.Creatures.Players;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Model;
using NeoServer.Game.World.Map.Tiles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Bases
{
   
    public abstract class CombatActor : WalkableCreature, ICombatActor
    {
        #region Events
        public event Heal OnHeal;
        public event StopAttack OnStoppedAttack;
        public event BlockAttack OnBlockedAttack;
        public event Attack OnAttack;
        public event Damage OnDamaged;
        public event Die OnKilled;
        public event OnAttackTargetChange OnTargetChanged;
        public event ChangeVisibility OnChangedVisibility;
        #endregion

        #region Properties
        public bool IsDead => HealthPoints <= 0;
        public decimal BaseAttackSpeed  => 2000M;
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
        private bool canBlock()
        {
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

        public ushort ReduceDamage(int attack)
        {
            int damage;

            if (canBlock())
            {
                damage = ShieldDefend(attack);

                if (damage <= 0)
                {
                    damage = 0;

                    block();
                    OnBlockedAttack?.Invoke(this, BlockType.Shield);
                    return (ushort)damage;
                }
            }

            damage = ArmorDefend(attack);

            if (damage <= 0)
            {
                damage = 0;
                OnBlockedAttack?.Invoke(this, BlockType.Armor);
            }

            return (ushort)damage;
        }

        public virtual void ReceiveAttack(ICombatActor enemy, ICombatAttack attack)
        {
            var damage = attack.CalculateDamage(enemy.AttackPower, enemy.MinimumAttackPower);
            ReceiveAttack(enemy, attack, damage);
        }
        public virtual void ReceiveAttack(ICombatActor enemy, ICombatAttack attack, ushort damage)
        {
            if (!attack.IsMagicalDamage)
            {
                damage = ReduceDamage(damage);
            }

            if (damage <= 0)
            {
                WasDamagedOnLastAttack = false;
                return;
            }

            if (IsDead)
            {
                return;
            }

            if (attack.DamageType != DamageType.ManaDrain)
            {
                ReduceHealth(damage);
            }

            OnDamaged?.Invoke(enemy, this, attack, damage);
            WasDamagedOnLastAttack = true;
            return;
        }

        public void StopAttack()
        {
            AutoAttackTargetId = 0;
            OnStoppedAttack?.Invoke(this);
        }
        public bool WasDamagedOnLastAttack = true;

        public virtual bool Attack(ICombatActor enemy, ICombatAttack combatAttack)
        {
            if (enemy.IsDead)
            {
                StopAttack();
                return false;
            }

            if (!(combatAttack is IDistanceCombatAttack) && !Tile.IsNextTo(enemy.Tile))
            {
                return false;
            }

            if (!Cooldowns.Expired(CooldownType.Combat)) return false;

            SetAttackTarget(enemy.CreatureId);

            combatAttack.BuildAttack(this, enemy);
            OnAttack?.Invoke(this, enemy, combatAttack);

            if (combatAttack.HasTarget)
            {
                combatAttack.CauseDamage(this, enemy);
            }

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
    }
}
