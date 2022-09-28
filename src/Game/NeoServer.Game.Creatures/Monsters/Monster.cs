using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NeoServer.Game.Combat;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Combat;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Creatures.Monsters.Combats;
using NeoServer.Game.Creatures.Monsters.Loots;

namespace NeoServer.Game.Creatures.Monsters;

public class Monster : WalkableMonster, IMonster
{
    private readonly ConcurrentDictionary<ICreature, ushort> damages; //todo: change for dictionary
    private Dictionary<string, byte> AliveSummons;
    private MonsterState state;

    public Monster(IMonsterType type, IMapTool mapTool, ISpawnPoint spawn) : base(type, mapTool)
    {
        if (type.IsNull()) return;

        Metadata = type;
        Spawn = spawn;
        damages = new ConcurrentDictionary<ICreature, ushort>();
        State = MonsterState.Sleeping;
        OnInjured += (enemy, _, damage) => RecordDamage(enemy, damage.Damage);
        Targets = new TargetList(this);
    }

    public byte TargetDistance =>
        Metadata.Flags.TryGetValue(CreatureFlagAttribute.TargetDistance, out var targetDistance)
            ? (byte)targetDistance
            : (byte)1;

    protected override string InspectionText => $"{IInspectionTextBuilder.GetArticle(Name)} {Name.ToLower()}.";
    protected override string CloseInspectionText => InspectionText;

    public bool KeepDistance => TargetDistance > 1;
    public IMonsterCombatAttack[] Attacks => Metadata.Attacks;
    public ICombatDefense[] Defenses => Metadata.Defenses;
    public virtual TargetList Targets { get; }

    public override FindPathParams PathSearchParams
    {
        get
        {
            var fpp = base.PathSearchParams;
            fpp.MaxTargetDist = TargetDistance;
            if (TargetDistance <= 1)
                fpp.FullPathSearch = true; //todo: needs to check if monster can attack from distance
            return fpp;
        }
    }

    public event Born OnWasBorn;
    public event MonsterChangeState OnChangedState;

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

    public ImmutableDictionary<ICreature, ushort> Damages => damages.ToImmutableDictionary();

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
        attack -= (ushort)GameRandom.Random.NextInRange(Defense / 2f, Defense);
        return attack;
    }

    public override int ArmorDefend(int attack)
    {
        switch (ArmorRating)
        {
            case > 3:
                attack -= (ushort)GameRandom.Random.NextInRange(ArmorRating / 2f, ArmorRating - (ArmorRating % 2 + 1));
                break;
            case > 0:
                --attack;
                break;
        }

        return attack;
    }

    public override bool ReceiveAttack(IThing enemy, CombatDamage damage)
    {
        if (enemy is Summon { Master: IPlayer } or IPlayer) return base.ReceiveAttack(enemy, damage);

        return false;
    }

    public override ushort ArmorRating => Metadata.Armor;

    public IMonsterType Metadata { get; }
    public override IOutfit Outfit { get; protected set; }
    public override ushort MinimumAttackPower => 0;
    public override bool UsingDistanceWeapon => TargetDistance > 1;
    public ISpawnPoint Spawn { get; }

    public bool IsHostile => Metadata.HasFlag(CreatureFlagAttribute.Hostile);

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
        if (creature is Monster { IsSummon: false }) return;
        if (creature is Summon summon && summon.Master.CreatureId == CreatureId) return;

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
            if (Targets.Any() &&
                Metadata.Flags.TryGetValue(CreatureFlagAttribute.RunOnHealth, out var runOnHealth) &&
                runOnHealth >= HealthPoints)
            {
                State = MonsterState.Running;
                return;
            }

            State = MonsterState.InCombat;
            return;
        }


        if (!Targets.Any() && Cooldowns.Expired(CooldownType.Awaken)) State = MonsterState.Sleeping;
    }

    public bool Defending { get; private set; }
    public virtual bool IsSummon => false;

    public override bool CanSeeInvisible => HasImmunity(Immunity.Invisibility); //todo: add invisibility flag

    public override bool CanBeSeen => false;

    public void MoveAroundEnemy()
    {
        if (!Targets.TryGetTarget(AutoAttackTargetId, out var combatTarget)) return;

        if (!IsInPerfectPositionToCombat(combatTarget)) return;

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
            foreach (CombatTarget target in Targets)
            {
                if (target.CanReachCreature)
                {
                    escapeFrom = target.Creature;
                    break;
                }

                escapeFrom = target.Creature;
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

        if (!Metadata.Voices.Any() ||
            Metadata.VoiceConfig.Chance < GameRandom.Random.Next(1, maxValue: 100)) return;

        var voiceIndex = GameRandom.Random.Next(0, maxValue: Metadata.Voices.Length - 1);

        var voice = Metadata.Voices[voiceIndex];
        Say(voice.Sentence, voice.SpeechType);
    }

    public void UpdateLastTargetChance()
    {
        if (!Cooldowns.Expired(CooldownType.TargetChange)) return;
        Cooldowns.Start(CooldownType.TargetChange, Metadata.TargetChance.Interval);
    }

    public ushort Defend()
    {
        if (IsDead || !Defenses.Any())
        {
            StopDefending();
            return default;
        }

        Defending = true;

        var defenseIndex = GameRandom.Random.Next(0, maxValue: Defenses.Length);
        var defense = Defenses[defenseIndex];

        if (defense.Chance < GameRandom.Random.Next(1, maxValue: 100))
            return defense.Interval; //can defend but lost his chance

        defense.Defend(this);

        return defense.Interval;
    }

    public void Summon(ISummonService summonService)
    {
        if ((AliveSummons?.Count ?? 0) >= Metadata.MaxSummons) return;

        foreach (var summon in Metadata.Summons)
        {
            if (!Cooldowns.Expired(summon.Name)) continue;

            if (summon.Chance < GameRandom.Random.Next(0, maxValue: 100))
                continue;

            byte count = 0;
            var foundAliveSummon = AliveSummons != null && AliveSummons.TryGetValue(summon.Name, out count);

            if (foundAliveSummon && count >= summon.Max) continue;

            var createdSummon = summonService.Summon(this, summon.Name);
            if (createdSummon is null) continue;

            Cooldowns.Start(summon.Name, (int)summon.Interval);

            AttachToSummonEvents(createdSummon);

            AliveSummons ??= new Dictionary<string, byte>();

            if (foundAliveSummon) AliveSummons[summon.Name] = (byte)(count + 1);
            else
                AliveSummons.TryAdd(summon.Name, 1);
        }
    }

    public void RecordDamage(IThing enemy, ushort damage)
    {
        if (enemy is not ICreature creature) return;
        damages.AddOrUpdate(creature, damage, (_, oldValue) => (ushort)(oldValue + damage));
    }

    public void Awake()
    {
        State = MonsterState.Awake;
        Cooldowns.Start(CooldownType.Awaken, 10000);
    }

    public bool IsInPerfectPositionToCombat(CombatTarget target)
    {
        if (KeepDistance)
        {
            if (target.Creature.Location.GetMaxSqmDistance(Location) == TargetDistance)
                return true && target.CanReachCreature;
        }
        else
        {
            if (target.Creature.Location.GetMaxSqmDistance(Location) <= TargetDistance)
                return true && target.CanReachCreature;
        }

        return false;
    }

    public override bool HasImmunity(Immunity immunity)
    {
        return (Metadata.Immunities & (ushort)immunity) != 0;
    }

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

            if (MapTool.PathFinder.Find(this, target.Creature.Location, PathSearchParams, TileEnterRule,
                    out var directions) == false)
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

    public void StopDefending()
    {
        Defending = false;
    }

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

            if (damage.Value == maxDamage) enemies.Add(damage.Key);
        }

        foreach (var enemy in enemies)
            if (enemy is IPlayer player && player.PlayerParty.Party is not null)
                partyMembers.AddRange(player.PlayerParty.Party.Members);
        return enemies.Concat(partyMembers).ToList();
    }

    public override bool OnAttack(ICombatActor enemy, out CombatAttackResult[] combatAttacks)
    {
        var arrayPool = ArrayPool<CombatAttackResult>.Shared;

        combatAttacks = arrayPool.Rent(Attacks.Length);

        if (!Attacks.Any()) return false;

        var attacked = false;

        var maxNumberOfAttacks = (int)Math.Min(3.0, Math.Ceiling(Attacks.Length / 1.5));
        var numberOfSuccessfulAttacks = 0;

        var comboChance = 70;

        foreach (var attack in Attacks)
        {
            if (!attack.Cooldown.Expired) continue;

            if (attack.Chance < GameRandom.Random.Next(0, maxValue: 100))
                continue;

            if (attack.CombatAttack is null)
            {
                Console.WriteLine($"Combat attack not found for monster: {Name}");
                continue;
            }

            if (attack.CombatAttack.TryAttack(this, enemy, attack.Translate(), out var combatAttack) is false) continue;

            combatAttacks[numberOfSuccessfulAttacks++] = combatAttack;

            attacked = true;

            if (comboChance < GameRandom.Random.Next(0, maxValue: 100) ||
                numberOfSuccessfulAttacks >= maxNumberOfAttacks)
                break; //chance to combo next attack

            comboChance = Math.Max(0, comboChance - 30);
        }

        if (attacked) TurnTo(Location.DirectionTo(enemy.Location));

        if (enemy.IsDead) Targets.RemoveTarget(enemy);

        arrayPool.Return(combatAttacks);
        combatAttacks = combatAttacks[..numberOfSuccessfulAttacks];


        return attacked;
    }

    public override CombatDamage OnImmunityDefense(CombatDamage damage)
    {
        if (damage.Damage <= 0) return damage;

        if (HasImmunity(damage.Type.ToImmunity()))
        {
            damage.SetNewDamage(0);
            return damage;
        }

        if (Metadata.ElementResistance is null) return damage;

        if (!Metadata.ElementResistance.TryGetValue(damage.Type, out var resistance)) return damage;

        var valueToReduce = Math.Round(damage.Damage * (decimal)(resistance / 100f));

        damage.IncreaseDamage((int)valueToReduce);

        return damage;
    }

    public override void OnDamage(IThing enemy, CombatDamage damage)
    {
        ReduceHealth(damage);
    }

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

    public override bool IsHostileTo(ICombatActor enemy)
    {
        return enemy is not IMonster && IsHostile;
    }

    #endregion
}