using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Game.Creatures.Model.Combat;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Helpers;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Creatures.Model.Monsters
{
    public class Monster : CombatActor, IMonster
    {
        public event Born OnWasBorn;
        public event Defende OnDefende;

        public Monster(IMonsterType type, PathFinder pathFinder, ISpawnPoint spawn) : base(type, pathFinder)
        {
            type.ThrowIfNull();
            spawn.ThrowIfNull();

            Metadata = type;
            Spawn = spawn;
            Damages = new ConcurrentDictionary<ICreature, ushort>();

            OnDamaged += (enemy, victim, combatAttack, damage) => RecordDamage(enemy, damage);
            OnKilled += (enemy) => GiveExperience();
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


        public void Reborn()
        {
            ResetHealthPoints();
            SetNewLocation(Spawn.Location);
            OnWasBorn?.Invoke(this, Spawn.Location);
        }

        public override int ShieldDefend(int attack)
        {

            attack -= (ushort)GaussianRandom.Random.NextInRange((Defense / 2), Defense);

            return attack;
        }

        public byte TargetDistance => Metadata.Flags[CreatureFlagAttribute.TargetDistance];
        public bool KeepDistance => TargetDistance > 1;

        public List<ICombatAttack> Attacks => Metadata.Attacks;
        public List<ICombatDefense> Defenses => Metadata.Defenses;

        public override int ArmorDefend(int attack)
        {
            if (ArmorRating > 3)
            {
                attack -= (ushort)GaussianRandom.Random.NextInRange(ArmorRating / 2, ArmorRating - (ArmorRating % 2 + 1));
            }
            else if (ArmorRating > 0)
            {
                --attack;
            }
            return attack;
        }

        public override ushort AttackPower
        {
            get
            {
                //if (Metadata.Attacks.)
                //{
                //    return (ushort)(Math.Ceiling((combatAttack.Skill * (combatAttack.Attack * 0.05)) + (combatAttack.Attack * 0.5)));
                //}

                return 0;
            }
        }

        /// <summary>
        /// Execute defense action
        /// </summary>
        /// <returns>interval</returns>
        public ushort Defende()
        {
            if (!Defenses.Any())
            {
                Defending = false;
                return default;
            }

            Defending = true;

            var defense = (ICombatDefense)ProbabilityRandom.Next(Defenses.ToArray()); //todo: remove allocation here

            defense?.Defende(this);

            if (defense != null)
            {
                OnDefende?.Invoke(this, defense);
            }

            return defense?.Interval == null ? Defenses[0].Interval : defense.Interval;


        }

        public override ushort ArmorRating => Metadata.Armor;

        public override byte AutoAttackRange => 0;

        public IMonsterType Metadata { get; }
        public override IOutfit Outfit { get; protected set; }

        public override ushort MinimumAttackPower => 0;

        public override bool UsingDistanceWeapon => TargetDistance > 1;

        public ISpawnPoint Spawn { get; }

        public override ushort DefensePower => 30;

        public ushort Defense => Metadata.Defence;

        public uint Experience => Metadata.Experience;

        public bool HasAnyTarget => Targets.Count > 0;

        public void SetState(MonsterState state) => State = state;

        private IDictionary<uint, CombatTarget> Targets = new Dictionary<uint, CombatTarget>(150);

        public void AddToTargetList(ICombatActor creature)
        {
            Targets.TryAdd(creature.CreatureId, new CombatTarget(creature));

            if (!Attacking) SelectTargetToAttack();
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
            if (CanReachAnyTarget) return false;

            int randomIndex = GaussianRandom.Random.Next(minValue: 0, maxValue: 4);

            var directions = new Direction[4] { Direction.East, Direction.North, Direction.South, Direction.West };

            TryWalkTo(directions[randomIndex]);

            return true;
        }

        public bool CanReachAnyTarget { get; private set; } = false;
        public bool IsInCombat => State == MonsterState.InCombat;
        public bool IsInPerfectPostionToCombat(CombatTarget target)
        {
            if (KeepDistance)
            {
                if (target.Creature.Location.GetSqmDistanceX(Location) == TargetDistance || target.Creature.Location.GetSqmDistanceY(Location) == TargetDistance)
                {
                    return true && target.CanReachCreature;
                }
            }
            else
            {
                if (target.Creature.Location.GetSqmDistanceX(Location) <= TargetDistance && target.Creature.Location.GetSqmDistanceY(Location) <= TargetDistance)
                {
                    return true && target.CanReachCreature;

                }
            }
            return false;
        }

        public bool Defending { get; private set; }

        private CombatTarget searchTarget()
        {
            //_findPathToDestination.ThrowIfNull();
            Targets.ThrowIfNull();

            var nearest = ushort.MaxValue;
            CombatTarget nearestCombat = null;

            var canReachAnyTarget = false;

            var fpp = new FindPathParams(!HasFollowPath, true, true, KeepDistance, 12, 1, TargetDistance, false);

            foreach (var target in Targets)
            {
                if (FindPathToDestination.Invoke(this, target.Value.Creature.Location, fpp, out var directions) == false)
                {
                    target.Value.SetAsUnreachable();
                    Console.WriteLine("UNREACHABLE");
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

        public void MoveAroundEnemy()
        {
            if(!Targets.TryGetValue(AutoAttackTargetId, out var combatTarget)) return;

            if (!Cooldowns.Expired(CooldownType.MoveAroundEnemy)) return;
            Cooldowns.Start(CooldownType.MoveAroundEnemy, GaussianRandom.Random.Next(minValue:3000, maxValue:5000));

            if (!IsInPerfectPostionToCombat(combatTarget)) return;

            if (FindPathToDestination(this, combatTarget.Creature.Location, new FindPathParams(HasFollowPath, true, true, KeepDistance, 12, 1, TargetDistance, true), out var directions))
            {
                TryWalkTo(directions);
            }
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
                SetState(MonsterState.InCombat);
            }

            FollowCreature = true;

            if (FollowCreature)
            {
                StartFollowing(target.Creature, new FindPathParams(HasFollowPath, true, true, KeepDistance, 12, 1, TargetDistance, false));
            }

            SetAttackTarget(target.Creature.CreatureId);
        }

        public override bool Attack(ICombatActor enemy, ICombatAttack combatAttack = null)
        {
            if ((Attacks?.Count ?? 0) == 0)
            {
                return false;
            }

            var index = GaussianRandom.Random.Next(minValue: 0, maxValue: Attacks.Count);

            TurnTo(Location.DirectionTo(enemy.Location));

            var attack = Attacks[index];

            return base.Attack(enemy, attack);
        }

    }
}
