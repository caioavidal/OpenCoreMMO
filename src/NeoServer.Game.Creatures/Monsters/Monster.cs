using NeoServer.Game.Combat;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Combat.Attacks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Model.Monsters.Loots;
using NeoServer.Game.Creatures.Monsters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Creatures.Model.Monsters
{
    public class Monster : WalkableMonster, IMonster
    {
        public event Born OnWasBorn;
        public event DropLoot OnDropLoot;
        public event MonsterChangeState OnChangedState;
        public Monster(IMonsterType type, IPathAccess pathAccess, ISpawnPoint spawn) : base(type, pathAccess)
        {
            type.ThrowIfNull();

            Metadata = type;
            Spawn = spawn;
            Damages = new ConcurrentDictionary<ICreature, ushort>();
            State = MonsterState.Sleeping;
            OnDamaged += (enemy, victim, damage) => RecordDamage(enemy, damage.Damage);
            OnKilled += (enemy) => GiveExperience();

        }

        private MonsterState state;
        public MonsterState State
        {
            get => state;
            private set
            {
                var oldState = state;
                if (state == value) return;
                state = value;
                OnChangedState?.Invoke(this, oldState, value);
            }
        }

        public ConcurrentDictionary<ICreature, ushort> Damages;

        public void RecordDamage(IThing enemy, ushort damage)
        {
            if (enemy is not ICreature creature) return;
            Damages.AddOrUpdate(creature, damage, (key, oldValue) => (ushort)(oldValue + damage));
        }
        
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

        public void Born(Location location)
        {
            Damages.Clear();
            ResetHealthPoints();
            Location = location;
            State = MonsterState.Sleeping;
            OnWasBorn?.Invoke(this, location);
        }
        public void Reborn()
        {
            if (Spawn is null) return;

            Damages.Clear();
            ResetHealthPoints();
            Location = Spawn.Location;
            State = MonsterState.Sleeping;
            OnWasBorn?.Invoke(this, Spawn.Location);
        }

        public override int ShieldDefend(int attack)
        {
            attack -= (ushort)GameRandom.Random.NextInRange((Defense / 2), Defense);
            return attack;
        }

        public byte TargetDistance => (byte)Metadata.Flags[CreatureFlagAttribute.TargetDistance];
        public bool KeepDistance => TargetDistance > 1;
        public IMonsterCombatAttack[] Attacks => Metadata.Attacks;
        public ICombatDefense[] Defenses => Metadata.Defenses;

        public override int ArmorDefend(int attack)
        {
            if (ArmorRating > 3)
            {
                attack -= (ushort)GameRandom.Random.NextInRange(ArmorRating / 2, ArmorRating - (ArmorRating % 2 + 1));
            }
            else if (ArmorRating > 0)
            {
                --attack;
            }
            return attack;
        }

        public override ushort ArmorRating => Metadata.Armor;

        public IMonsterType Metadata { get; }
        public override IOutfit Outfit { get; protected set; }
        public override ushort MinimumAttackPower => 0;
        public override bool UsingDistanceWeapon => TargetDistance > 1;
        public ISpawnPoint Spawn { get; }
        public override BloodType Blood => Metadata.Race switch
        {
            Race.Bood => BloodType.Blood,
            Race.Venom => BloodType.Slime,
            _ => BloodType.Blood
        };
        public ushort Defense => Metadata.Defense;
        public uint Experience => Metadata.Experience;
        private IDictionary<uint, CombatTarget> Targets = new Dictionary<uint, CombatTarget>(150);

        public void AddToTargetList(ICombatActor creature)
        {
            Targets.TryAdd(creature.CreatureId, new CombatTarget(creature));
        }
        public void RemoveFromTargetList(ICreature creature)
        {
            Targets.Remove(creature.CreatureId);

            if (AutoAttackTargetId == creature.CreatureId) StopAttack();
        }

        public override void SetAsEnemy(ICreature creature)
        {
            if (creature is not ICombatActor enemy) return;
            if (creature is Monster monster && !monster.IsSummon) return;
            if (!enemy.CanBeAttacked) return;

            var canSee = CanSee(creature.Location, 9, 7);

            if (State == MonsterState.Sleeping)
                Awake();

            if (IsDead || creature.IsRemoved || !canSee)
            {
                RemoveFromTargetList(creature);
                return;
            }

            AddToTargetList(enemy);
        }

        public bool IsInCombat => State == MonsterState.InCombat;
        public bool IsSleeping => State == MonsterState.Sleeping;

        public void Awake()
        {
            State = MonsterState.Awake;
            Cooldowns.Start(CooldownType.Awaken, 10000);
        }

        public void ChangeState()
        {
            searchTarget();

            if (!Targets.Any() && Cooldowns.Expired(CooldownType.Awaken))
            {
                State = MonsterState.Sleeping;
                return;
            }
         
            if (Targets.Any() && !CanReachAnyTarget)
            {
                State = MonsterState.LookingForEnemy;
                return;
            }
            if (Targets.Any() && CanReachAnyTarget)
            {
                if (Targets.Any() && Metadata.Flags.TryGetValue(CreatureFlagAttribute.RunOnHealth, out var runOnHealth) && runOnHealth >= HealthPoints)
                {
                    State = MonsterState.Running;
                    return;
                }

                State = MonsterState.InCombat;
                return;
            }

            if (!Targets.Any()) State = MonsterState.Sleeping;
        }
        public bool IsInPerfectPostionToCombat(CombatTarget target)
        {
            if (KeepDistance)
            {
                if (target.Creature.Location.GetMaxSqmDistance(Location) == TargetDistance) return true && target.CanReachCreature;
            }
            else
            {
                if (target.Creature.Location.GetMaxSqmDistance(Location) <= TargetDistance) return true && target.CanReachCreature;
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
                if (target.Value.Creature.IsDead || target.Value.Creature.IsRemoved)
                {
                    RemoveFromTargetList(target.Value.Creature);
                    continue;
                }

                if (PathAccess.FindPathToDestination.Invoke(this, target.Value.Creature.Location, PathSearchParams, CreatureEnterTileRule.Rule, out var directions) == false)
                {
                    target.Value.SetAsUnreachable();
                    continue;
                }

                if (target.Value.Creature.IsInvisible && !CanSeeInvisible)
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
            if (!Targets.TryGetValue(AutoAttackTargetId, out var combatTarget)) return;

            if (!IsInPerfectPostionToCombat(combatTarget)) return;

            MoveAroundEnemy(combatTarget.Creature.Location);
        }

        public void SelectTargetToAttack()
        {
            if (Attacking && !Cooldowns.Cooldowns[CooldownType.TargetChange].Expired) return;

            var target = searchTarget();

            if (target is null) return;

            FollowCreature = Speed > 0;

            if (FollowCreature)
            {
                StartFollowing(target.Creature, PathSearchParams);
            }

            SetAttackTarget(target.Creature);
            UpdateLastTargetChance();
        }

        public void Sleep()
        {
            State = MonsterState.Sleeping;

            StopAttack();
            StopFollowing();
        }
        public void Escape()
        {
            StopFollowing();
            StopAttack();

            ICreature escapeFrom = null;

            if (Targets.TryGetValue(AutoAttackTargetId, out var creature)) escapeFrom = creature.Creature;
            else
            {
                foreach (var target in Targets.Values)
                {
                    if (target.CanReachCreature)
                    {
                        escapeFrom = target.Creature;
                        break;
                    }
                    escapeFrom = target.Creature;
                }
            }
            if (escapeFrom is null) return;

            Escape(escapeFrom.Location);
        }

        public void Yell()
        {
            if (Metadata.Voices is null) return;
            if (Metadata.VoiceConfig is null) return;

            if (!Cooldowns.Expired(CooldownType.Yell)) return;
            Cooldowns.Start(CooldownType.Yell, Metadata.VoiceConfig.Interval);

            if (!Metadata.Voices.Any() || Metadata.VoiceConfig.Chance < GameRandom.Random.Next(minValue: 1, maxValue: 100)) return;

            var voiceIndex = GameRandom.Random.Next(minValue: 0, maxValue: Metadata.Voices.Length - 1);

            Say(Metadata.Voices[voiceIndex], SpeechType.MonsterYell);
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

            DropLoot();
        }
        public void DropLoot()
        {
            var loot = Metadata.Loot?.Drop();
            OnDropLoot?.Invoke(this, new Loot(loot));
        }
        public ushort Defend()
        {
            if (IsDead || !Defenses.Any())
            {
                StopDefending();
                return default;
            }

            Defending = true;

            var defenseIndex = GameRandom.Random.Next(minValue: 0, maxValue: Defenses.Length);
            var defense = Defenses[defenseIndex];

            if (defense.Chance < GameRandom.Random.Next(minValue: 1, maxValue: 100)) return defense.Interval; //can defend but lost his chance

            defense.Defende(this);

            return defense.Interval;
        }

        public override bool OnAttack(ICombatActor enemy, out CombatAttackType combat)
        {
            combat = new CombatAttackType();


            if (!Attacks.Any()) return false;

            var attacked = false;

            foreach (var attack in Attacks)
            {
                if (!attack.Cooldown.Expired) continue;

                if (attack.Chance < GameRandom.Random.Next(minValue: 0, maxValue: 100))
                    continue;

                if (attack.CombatAttack is null) Console.WriteLine($"Combat attack not found for monster: {Name}");

                if (!(attack.CombatAttack?.TryAttack(this, enemy, attack.Translate(), out combat) ?? false)) continue;
                attacked = true;
            }

            if(attacked) TurnTo(Location.DirectionTo(enemy.Location));

            if (enemy.IsDead || enemy.IsRemoved) RemoveFromTargetList(enemy);

            return attacked;
        }

        public override CombatDamage OnImmunityDefense(CombatDamage damage)
        {
            if (damage.Damage <= 0) return damage;

            if (!Metadata.Immunities.ContainsKey(damage.Type)) return damage;

            var valueToReduce = Math.Round(damage.Damage * (decimal)(Metadata.Immunities[damage.Type] / 100f));

            damage.IncreaseDamage((int)valueToReduce);

            return damage;
        }

        public override void OnDamage(IThing enemy, CombatDamage damage) => ReduceHealth( damage);
        
    }
}
