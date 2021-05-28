using NeoServer.Game.Combat;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Services;
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
using NeoServer.Game.Creatures.Monsters.Combats;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NeoServer.Game.Creatures.Model.Monsters
{
    public class Monster : WalkableMonster, IMonster
    {
        public event Born OnWasBorn;
        public event MonsterChangeState OnChangedState;
        public Monster(IMonsterType type, ISpawnPoint spawn) : base(type)
        {
            if (type.IsNull()) return;

            Metadata = type;
            Spawn = spawn;
            damages = new ConcurrentDictionary<ICreature, ushort>();
            State = MonsterState.Sleeping;
            OnDamaged += (enemy, victim, damage) => RecordDamage(enemy, damage.Damage);
            Targets = new TargetList(this);
        }

        private Dictionary<string, byte> AliveSummons;
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

        private ConcurrentDictionary<ICreature, ushort> damages; //todo: change for dictionary

        public ImmutableDictionary<ICreature, ushort> Damages => damages.ToImmutableDictionary();

        public void RecordDamage(IThing enemy, ushort damage)
        {
            if (enemy is not ICreature creature) return;
            damages.AddOrUpdate(creature, damage, (key, oldValue) => (ushort)(oldValue + damage));
        }

        public void Born(Location location)
        {
            damages.Clear();
            ResetHealthPoints();
            Location = location;
            State = MonsterState.Sleeping;
            OnWasBorn?.Invoke(this, location);
        }
        public void Reborn()
        {
            if (Spawn is null) return;

            damages.Clear();
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
        public virtual TargetList Targets { get; private set; }

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
        public override BloodType BloodType => Metadata.Race switch
        {
            Race.Bood => BloodType.Blood,
            Race.Venom => BloodType.Slime,
            _ => BloodType.Blood
        };
        public ushort Defense => Metadata.Defense;
        public uint Experience => Metadata.Experience;



        public override void SetAsEnemy(ICreature creature)
        {
            if (creature is not ICombatActor enemy) return;
            if (creature is Monster monster && !monster.IsSummon) return;
            if (creature is Summon summon && summon.Master.CreatureId == this.CreatureId) return;

            if (!enemy.CanBeAttacked) return;

            var canSee = CanSee(creature.Location, 9, 7);

            if (State == MonsterState.Sleeping)
                Awake();

            if (IsDead || !canSee)
            {
                Targets.RemoveTarget(creature);
                return;
            }

            Targets.AddTarget(enemy);
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
            SearchTarget();

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
        public bool HasImmunity(Immunity immunity) => (Metadata.Immunities & (ushort)immunity) != 0;
        public virtual bool IsSummon => false;

        public override bool CanSeeInvisible => HasImmunity(Immunity.Invisibility); //todo: add invisibility flag

        public override bool CanBeSeen => false;

        public override void OnCreatureDisappear(ICreature creature)
        {
            Targets.RemoveTarget(creature);
            SelectTargetToAttack();
        }

        protected virtual CombatTarget SearchTarget()
        {
            if (Targets.IsNull()) return null;

            var nearest = ushort.MaxValue;
            CombatTarget nearestCombat = null;

            var canReachAnyTarget = false;

            foreach (CombatTarget target in Targets)
            {
                if (target.Creature.IsDead)
                {
                    Targets.RemoveTarget(target.Creature);
                    continue;
                }

                if (PathFinder.Find(this, target.Creature.Location, PathSearchParams, TileEnterRule, out var directions) == false)
                {
                    target.SetAsUnreachable();
                    continue;
                }

                if (target.Creature.IsInvisible && !CanSeeInvisible)
                {
                    target.SetAsUnreachable();
                    continue;
                }

                target.CanSee = true;

                target.SetAsReachable(directions);

                canReachAnyTarget = true;

                var offset = Location.GetSqmDistance(target.Creature.Location);
                if (offset < nearest)
                {
                    nearest = offset;
                    nearestCombat = target;
                }
            }
            CanReachAnyTarget = canReachAnyTarget;

            return nearestCombat;
        }

        public void MoveAroundEnemy()
        {
            if (!Targets.TryGetTarget(AutoAttackTargetId, out var combatTarget)) return;

            if (!IsInPerfectPostionToCombat(combatTarget)) return;

            MoveAroundEnemy(combatTarget.Creature.Location);
        }

        public virtual void SelectTargetToAttack()
        {
            if (Attacking && !Cooldowns.Cooldowns[CooldownType.TargetChange].Expired) return;

            var target = SearchTarget();

            if (target is null) return;

            Follow(target.Creature);
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

            if (Targets.TryGetTarget(AutoAttackTargetId, out var creature)) escapeFrom = creature.Creature;
            else
            {
                foreach (CombatTarget target in Targets)
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

        public override void OnDeath(IThing by)
        {
            Targets?.Clear();
            StopAttack();

            StopDefending();
            base.OnDeath(by);
        }
        public override ILoot DropLoot()
        {
            var lootItems = Metadata.Loot?.Drop();

            var enemies = GetLootOwners();

            var loot = new Loot(lootItems, enemies.ToHashSet());

            return loot;
        }

        private List<ICreature> GetLootOwners()
        {
            var enemies = new List<ICreature>();
            var partyMembers = new List<ICreature>();

            ushort maxDamage = 0;

            foreach (var damage in damages)
            {
                if (damage.Value > maxDamage)
                {
                    enemies.Clear();
                    enemies.Add(damage.Key);
                    maxDamage = damage.Value;
                }
                if (damage.Value == maxDamage)
                {
                    enemies.Add(damage.Key);
                }
            }

            foreach (var enemy in enemies)
            {
                if (enemy is IPlayer player && player.Party is not null)
                {
                    partyMembers.AddRange(player.Party.Members);
                }
            }
            return enemies.Concat(partyMembers).ToList();
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

                if (40 < GameRandom.Random.Next(minValue: 0, maxValue: 100)) break; //chance to combo next attack

            }

            if (attacked) TurnTo(Location.DirectionTo(enemy.Location));

            if (enemy.IsDead) Targets.RemoveTarget(enemy);

            return attacked;
        }

        public override CombatDamage OnImmunityDefense(CombatDamage damage)
        {
            if (damage.Damage <= 0) return damage;

            if (!Metadata.ElementResistance.ContainsKey(damage.Type)) return damage;

            var valueToReduce = Math.Round(damage.Damage * (decimal)(Metadata.ElementResistance[damage.Type] / 100f));

            damage.IncreaseDamage((int)valueToReduce);

            return damage;
        }

        public void Summon(ISummonService summonService)
        {
            if ((AliveSummons?.Count ?? 0) >= Metadata.MaxSummons) return;

            foreach (var summon in Metadata.Summons)
            {
                if (!Cooldowns.Expired(summon.Name)) continue;

                if (summon.Chance < GameRandom.Random.Next(minValue: 0, maxValue: 100))
                    continue;

                byte count = 0;
                var foundAliveSummon = AliveSummons != null && AliveSummons.TryGetValue(summon.Name, out count);

                if (foundAliveSummon && count >= summon.Max) continue;

                var createdSummon = summonService.Summon(this, summon.Name);
                if (createdSummon is null) continue;

                Cooldowns.Start(summon.Name, (int)summon.Interval);

                AttachToSummonEvents(createdSummon);

                AliveSummons = AliveSummons ?? new();

                if (foundAliveSummon) AliveSummons[summon.Name] = (byte)(count + 1);
                else
                {
                    AliveSummons.TryAdd(summon.Name, 1);
                }
            }
        }

        public override void OnDamage(IThing enemy, CombatDamage damage) => ReduceHealth(damage);

        #region Summon Event Attachment

        public void AttachToSummonEvents(IMonster monster)
        {
            monster.OnKilled += OnSummonDie;
        }

        private void OnSummonDie(ICombatActor creature, IThing by, ILoot loot)
        {
            creature.OnKilled -= OnSummonDie;
            if (!AliveSummons.TryGetValue(creature.Name, out var count)) return;

            if (count == 1)
            {
                AliveSummons.Remove(creature.Name);
                return;
            }

            AliveSummons[creature.Name] = (byte)(count - 1);
        }

        #endregion
    }
}
