using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Model.Combat;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Creatures.Model.Monsters
{
    public class Monster : Creature, IMonster
    {
        public delegate bool PathFinder(ICreature creature, Location target, out Direction[] directions);

        private PathFinder findPathToDestination;

        public Monster(IMonsterType type, ISpawnPoint spawn, PathFinder pathFinder) : base(type)
        {
            pathFinder.ThrowIfNull();
            type.ThrowIfNull();
            spawn.ThrowIfNull();

            findPathToDestination = pathFinder;
            Metadata = type;
            Spawn = spawn;
            Damages = new ConcurrentDictionary<ICreature, ushort>();

            OnDamaged += (enemy, victim, damage) => RecordDamage(enemy, damage);
            OnKilled += (enemy) => GiveExperience();

            Cooldowns[CooldownType.LookForNewEnemy] = new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.Zero);
        }

        public MonsterState State { get; private set; } = MonsterState.Sleeping;

        public ConcurrentDictionary<ICreature, ushort> Damages;

        public void RecordDamage(ICreature enemy, ushort damage) => Damages.AddOrUpdate(enemy, damage, (key, oldValue) => (ushort)(oldValue + damage));

        private void GiveExperience()
        {
            var totalDamage = Damages.Sum(x => x.Value);

            foreach (var enemyDamage in Damages)
            {
                var damage = enemyDamage.Value;

                var damagePercent = damage * 100 / totalDamage;

                var exp = damagePercent * Experience / 100;

                enemyDamage.Key.GainExperience((uint)exp);
            }
        }

        public event Born OnWasBorn;

        public void Reborn()
        {
            ResetHealthPoints();
            SetNewLocation(Spawn.Location);
            OnWasBorn?.Invoke(this, Spawn.Location);
        }

        public override int ShieldDefend(int attack)
        {

            attack -= RandomDamagePower((Defense / 2), Defense);

            return attack;
        }

        public override int ArmorDefend(int attack)
        {
            if (ArmorRating > 3)
            {
                attack -= RandomDamagePower(ArmorRating / 2, ArmorRating - (ArmorRating % 2 + 1));
            }
            else if (ArmorRating > 0)
            {
                --attack;
            }
            return attack;
        }

        public new ushort Speed => Metadata.Speed;

        public override ushort AttackPower
        {
            get
            {
                if (Metadata.Attacks.TryGetValue(Game.Enums.Item.DamageType.Melee, out ICombatAttack combatAttack))
                {
                    return (ushort)(Math.Ceiling((combatAttack.Skill * (combatAttack.Attack * 0.05)) + (combatAttack.Attack * 0.5)));
                }

                return 0;
            }
        }

        public override ushort ArmorRating => Metadata.Armor;

        public override byte AutoAttackRange => 0;

        public IMonsterType Metadata { get; }
        public override IOutfit Outfit { get; protected set; }

        public override ushort MinimumAttackPower => 0;

        public override bool UsingDistanceWeapon => false;

        public ISpawnPoint Spawn { get; }

        public override ushort DefensePower => 30;

        public ushort Defense => Metadata.Defence;

        public uint Experience => Metadata.Experience;

        public bool HasAnyTarget => Targets.Count > 0;

        public void SetState(MonsterState state) => State = state;

        private IDictionary<uint, CombatTarget> Targets = new Dictionary<uint, CombatTarget>(150);

     
      
        public void AddToTargetList(ICreature creature)
        {
            Targets.TryAdd(creature.CreatureId, new CombatTarget(creature));
        }
        public void RemoveFromTargetList(ICreature creature)
        {
            Targets.Remove(creature.CreatureId);

            if (AutoAttackTargetId == creature.CreatureId)
            {
                StopAttack();
            }
        }

        

        public bool LookForNewEnemy()
        {
            if(CalculateRemainingCooldownTime(CooldownType.LookForNewEnemy, DateTime.Now) > 0) return false;

            if (CanReachAnyTarget) return false;
            
            int randomIndex =_random.Next(minValue:0, maxValue: 4);

            var directions = new Direction[4] { Direction.East, Direction.North, Direction.South, Direction.West };

            TryWalkTo(directions[randomIndex]);

            RestartCoolDown(CooldownType.LookForNewEnemy, 2000);

            return true;
        }

        public bool CanReachAnyTarget { get; private set; } = false;

        private CombatTarget searchTarget()
        {
            findPathToDestination.ThrowIfNull();
            Targets.ThrowIfNull();

            var nearest = ushort.MaxValue;
            CombatTarget nearestCombat = null;

            var canReachAnyTarget = false;

            foreach (var target in Targets)
            {
                if(findPathToDestination.Invoke(this, target.Value.Creature.Location, out var directions) == false)
                {
                    target.Value.SetAsUnreachable();
                    continue;
                }

                target.Value.SetAsReachable(directions);

                canReachAnyTarget = true;

                var offset = Location.GetSqmDistance(target.Value.Creature.Location);
                if (offset < nearest)
                {
                    nearest = offset;
                    nearestCombat = target.Value;
                }
            }

            CanReachAnyTarget = canReachAnyTarget;

            return nearestCombat;
        }

        public void SelectTargetToAttack()
        {
            var target = searchTarget();

            if (target == null && !CanReachAnyTarget)
            {
                LookForNewEnemy();
                return;
            }

            if (target != null)
            {
                SetState(MonsterState.Alive);
            }

            FollowCreature = true;

            if (FollowCreature)
            {
                StartFollowing(target.Creature.CreatureId, target.PathToCreature);
            }

            SetAttackTarget(target.Creature.CreatureId);
        }
    }
}
