using NeoServer.Game.Combat;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Combat.Attacks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Talks;
using NeoServer.Server.Helpers;
using System;
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

            OnDamaged += (enemy, victim, damage) => RecordDamage(enemy, damage.Damage);
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
            Location = Spawn.Location;
            OnWasBorn?.Invoke(this, Spawn.Location);
        }

        public override int ShieldDefend(int attack)
        {
            attack -= (ushort)ServerRandom.Random.NextInRange((Defense / 2), Defense);
            return attack;
        }

        public byte TargetDistance => Metadata.Flags[CreatureFlagAttribute.TargetDistance];
        public bool KeepDistance => TargetDistance > 1;

        public IMonsterCombatAttack[] Attacks => Metadata.Attacks;
        public ICombatDefense[] Defenses => Metadata.Defenses;

        public override int ArmorDefend(int attack)
        {
            if (ArmorRating > 3)
            {
                attack -= (ushort)ServerRandom.Random.NextInRange(ArmorRating / 2, ArmorRating - (ArmorRating % 2 + 1));
            }
            else if (ArmorRating > 0)
            {
                --attack;
            }
            return attack;
        }

        public override ushort AttackPower => 0;

        public override ushort ArmorRating => Metadata.Armor;

        public override byte AutoAttackRange => 0;

        public IMonsterType Metadata { get; }
        public override IOutfit Outfit { get; protected set; }
        public override ushort MinimumAttackPower => 0;

        public override bool UsingDistanceWeapon => TargetDistance > 1;

        public ISpawnPoint Spawn { get; }

        public override ushort DefensePower => 30;
        public override BloodType Blood =>  Metadata.Race switch
        {
            Race.Bood => BloodType.Blood,
            Race.Venom => BloodType.Slime,
            _ => BloodType.Blood
        };

        public ushort Defense => Metadata.Defense;

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
            StopFollowing();
            if (CanReachAnyTarget) return false;

            int randomIndex = ServerRandom.Random.Next(minValue: 0, maxValue: 4);

            var directions = new Direction[4] { Direction.East, Direction.North, Direction.South, Direction.West };

            TryWalkTo(directions[randomIndex]);

            return true;
        }


        public void SetAsEnemy(ICombatActor creature)
        {
            if (!CanSee(creature.Location, 9, 9))
            {
                RemoveFromTargetList(creature);
                return;
            }

            creature.SetAsInFight();
            AddToTargetList(creature);
        }

        public bool CanReachAnyTarget { get; private set; } = false;
        public bool IsInCombat => State == MonsterState.InCombat;
        public bool IsSleeping => State == MonsterState.Sleeping;
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
        public override FindPathParams PathSearchParams
        {
            get
            {
                var fpp = base.PathSearchParams;
                fpp.MaxTargetDist = TargetDistance;
                if (TargetDistance <= 1) fpp.FullPathSearch = true; //todo: needs to check if mosnter can attack from distance
                return fpp;
            }
        }

        public bool IsSummon => false;

        public override void OnCreatureDisappear(ICreature creature)
        {
            RemoveFromTargetList(creature);
            SelectTargetToAttack();
        }

        private CombatTarget searchTarget()
        {
            Targets.ThrowIfNull();

            var nearest = ushort.MaxValue;
            CombatTarget nearestCombat = null;

            var canReachAnyTarget = false;

            foreach (var target in Targets)
            {
                if (FindPathToDestination.Invoke(this, target.Value.Creature.Location, PathSearchParams, out var directions) == false)
                {
                    target.Value.SetAsUnreachable();
                    continue;
                }

                target.Value.CanSee = true;

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
            return;
            if (!Targets.TryGetValue(AutoAttackTargetId, out var combatTarget)) return;

            if (!Cooldowns.Expired(CooldownType.MoveAroundEnemy)) return;
            Cooldowns.Start(CooldownType.MoveAroundEnemy, ServerRandom.Random.Next(minValue: 3000, maxValue: 5000));

            if (!IsInPerfectPostionToCombat(combatTarget)) return;

            if (FindPathToDestination(this, combatTarget.Creature.Location, new FindPathParams(HasFollowPath, true, true, KeepDistance, 12, 1, TargetDistance, true), out var directions))
            {
                TryWalkTo(directions);
            }
        }

        public void SelectTargetToAttack()
        {
            if (!Targets.Any())
            {
                Sleep();
                return;
            }

            if (Attacking && !Cooldowns.Cooldowns[CooldownType.TargetChange].Expired) return;

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
                StartFollowing(target.Creature, PathSearchParams);
            }

            SetAttackTarget(target.Creature.CreatureId);
            UpdateLastTargetChance();
        }

        public void Sleep()
        {
            State = MonsterState.Sleeping;
            StopAttack();
            StopFollowing();
        }

        public void Yell()
        {
            if (Metadata.Voices is null) return;
            if (Metadata.VoiceConfig is null) return;

            if (!Cooldowns.Expired(CooldownType.Yell)) return;
            Cooldowns.Start(CooldownType.Yell, Metadata.VoiceConfig.Interval);

            if (!Metadata.Voices.Any() || Metadata.VoiceConfig.Chance < ServerRandom.Random.Next(minValue: 1, maxValue: 100)) return;

            var voiceIndex = ServerRandom.Random.Next(minValue: 0, maxValue: Metadata.Voices.Length - 1);

            Say(Metadata.Voices[voiceIndex], TalkType.MonsterYell);
        }

        public void UpdateLastTargetChance()
        {
            if (!Cooldowns.Expired(CooldownType.TargetChange)) return;
            Cooldowns.Start(CooldownType.TargetChange, Metadata.TargetChance.Interval);
        }
        public void StopDefending() => Defending = false;

        public override void OnDeath()
        {
            Targets?.Clear();
            StopDefending();
            base.OnDeath();
        }
        public ushort Defend()
        {
            if (IsDead || !Defenses.Any())
            {
                StopDefending();
                return default;
            }

            Defending = true;

            var defenseIndex = ServerRandom.Random.Next(minValue: 0, maxValue: Defenses.Length);
            var defense = Defenses[defenseIndex];

            if (defense.Chance < ServerRandom.Random.Next(minValue: 1, maxValue: 100)) return defense.Interval; //can defend but lost his chance

            defense.Defende(this);

            OnDefende?.Invoke(this, defense);

            return defense.Interval;
        }

        public override bool OnAttack(ICombatActor enemy, out CombatAttackType combat)
        {
            combat = new CombatAttackType();

            if (!Attacks.Any()) return false;

            foreach (var attack in Attacks)
            {
                if (!attack.Cooldown.Expired) continue;

                if (attack.Chance < ServerRandom.Random.Next(minValue: 0, maxValue: 100)) continue;

                attack.CombatAttack.TryAttack(this, enemy, attack.Translate(), out combat);
            }

            TurnTo(Location.DirectionTo(enemy.Location));
            return true;
        }

        public override CombatDamage OnImmunityDefense(CombatDamage damage)
        {
            if (damage.Damage <= 0) return damage;

            if (!Metadata.Immunities.ContainsKey(damage.Type)) return damage;

            var valueToReduce = Math.Round(damage.Damage * (decimal)(Metadata.Immunities[damage.Type] / 100f));

            damage.IncreaseDamage((int)valueToReduce);

            return damage;
        }
    }
}
