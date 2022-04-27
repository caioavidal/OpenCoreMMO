using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Combat.Conditions;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Creatures.Players;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Common.Texts;
using NeoServer.Game.Creatures.Model.Bases;

namespace NeoServer.Game.Creatures.Model.Players;

public class Player : CombatActor, IPlayer
{
    private const int KNOWN_CREATURE_LIMIT = 250; //todo: for version 8.60

    private ulong _flags;

    private uint _idleTime;
    private byte _soulPoints;

    public Player(uint id, string characterName, ChaseMode chaseMode, uint capacity, uint healthPoints,
        uint maxHealthPoints, IVocation vocation,
        Gender gender, bool online, ushort mana, ushort maxMana, FightMode fightMode, byte soulPoints, byte soulMax,
        IDictionary<SkillType, ISkill> skills, ushort staminaMinutes,
        IOutfit outfit, ushort speed,
        Location location, IMapTool mapTool)
        : base(
            new CreatureType(characterName, string.Empty, maxHealthPoints, speed,
                new Dictionary<LookType, ushort> { { LookType.Corpse, 3058 } }), mapTool, outfit, healthPoints)
    {
        Id = id;
        CharacterName = characterName;
        ChaseMode = chaseMode;
        TotalCapacity = capacity;
        Skills = skills;
        Vocation = vocation;
        Gender = gender;
        Online = online;
        Mana = mana;
        MaxMana = maxMana;
        FightMode = fightMode;
        MaxSoulPoints = soulMax;
        SoulPoints = soulPoints;
        StaminaMinutes = staminaMinutes;
        Outfit = outfit;
        Speed = speed == 0 ? LevelBasesSpeed : speed;
        Inventory = new Inventory(this, new Dictionary<Slot, Tuple<IPickupable, ushort>>());

        Vip = new Vip(this);
        Channels = new PlayerChannel(this);
        PlayerParty = new PlayerParty(this);
        PlayerHand = new PlayerHand(this);

        Location = location;

        Containers = new PlayerContainerList(this);

        KnownCreatures = new Dictionary<uint, long>(); //todo

        foreach (var skill in Skills.Values)
        {
            skill.OnAdvance += OnLevelAdvance;
            skill.OnIncreaseSkillPoints += skill => OnGainedSkillPoint?.Invoke(this, skill);
        }
    }

    protected override string CloseInspectionText => InspectionText;

    protected override string InspectionText =>
        $"{Name} (Level {Level}). He is a {Vocation.Name.ToLower()}. {GuildText}";

    private ushort LevelBasesSpeed => (ushort)(220 + 2 * (Level - 1));
    public string CharacterName { get; }
    public Dictionary<uint, long> KnownCreatures { get; }
    public Gender Gender { get; }
    public bool Online { get; }

    public float DamageFactor => FightMode switch
    {
        FightMode.Attack => 1,
        FightMode.Balanced => 0.75f,
        FightMode.Defense => 0.5f,
        _ => 0.75f
    };

    public int DefenseFactor => FightMode switch
    {
        FightMode.Attack => 5,
        FightMode.Balanced => 7,
        FightMode.Defense => 10,
        _ => 7
    };

    public bool IsPacified => Conditions.ContainsKey(ConditionType.Pacified);

    private IDictionary<SkillType, ISkill> Skills { get; }

    public IVip Vip { get; }
    public override IOutfit Outfit { get; protected set; }
    public IVocation Vocation { get; }
    public IPlayerChannel Channels { get; set; }
    public IPlayerParty PlayerParty { get; set; }
    public IPlayerHand PlayerHand { get; }
    public ulong BankAmount { get; private set; }

    public ulong GetTotalMoney(ICoinTypeStore coinTypeStore)
    {
        return BankAmount + Inventory.GetTotalMoney(coinTypeStore);
    }


    public void LoadBank(ulong amount)
    {
        BankAmount = amount;
    }

    public uint AccountId { get; init; }
    public IPlayerContainerList Containers { get; }
    public bool HasDepotOpened => Containers.HasAnyDepotOpened;
    public IShopperNpc TradingWithNpc { get; private set; }
    public ChaseMode ChaseMode { get; private set; }
    public uint TotalCapacity { get; private set; }
    public ushort Level => (ushort)(Skills.TryGetValue(SkillType.Level, out var level) ? level?.Level ?? 1 : 1);
    public ushort Mana { get; private set; }
    public ushort MaxMana { get; private set; }
    public FightMode FightMode { get; private set; }

    public bool Shopping => TradingWithNpc is not null;

    public byte SoulPoints
    {
        get => _soulPoints;
        private set => _soulPoints = value > MaxSoulPoints ? MaxSoulPoints : value;
    }

    public byte MaxSoulPoints { get; }

    public IInventory Inventory { get; private set; }
    public ushort StaminaMinutes { get; }

    public uint Experience
    {
        get
        {
            if (Skills.TryGetValue(SkillType.Level, out var skill)) return (uint)skill.Count;
            return 0;
        }
    }

    public void AddInventory(IInventory inventory)
    {
        Inventory = inventory;
    }

    public byte LevelPercent => GetSkillPercent(SkillType.Level);

    public override void GainExperience(uint exp)
    {
        if (exp == 0) return;

        IncreaseSkillCounter(SkillType.Level, exp);
        base.GainExperience(exp);
    }

    public virtual bool CannotLogout => !(Tile?.ProtectionZone ?? false) && InFight;

    public SkillType SkillInUse
    {
        get
        {
            if (Inventory.Weapon is { } weapon)
                return weapon.Type switch
                {
                    WeaponType.Club => SkillType.Club,
                    WeaponType.Sword => SkillType.Sword,
                    WeaponType.Axe => SkillType.Axe,
                    WeaponType.Ammunition => SkillType.Distance,
                    WeaponType.Distance => SkillType.Distance,
                    WeaponType.Magical => SkillType.Magic,
                    _ => SkillType.Fist
                };
            return SkillType.Fist;
        }
    }

    public ushort CalculateAttackPower(float attackRate, ushort attack)
    {
        var damageMultiplier = SkillInUse switch
        {
            SkillType.Distance => Vocation.Formula?.DistDamage ?? 1f,
            SkillType.Magic => 1f,
            _ => Vocation.Formula?.MeleeDamage ?? 1f
        };
        return (ushort)(attackRate * DamageFactor * attack * Skills[SkillInUse].Level + Level / 5 * damageMultiplier);
    }

    public uint Id { get; }
    public override ushort MinimumAttackPower => (ushort)(Level / 5);
    public override ushort ArmorRating => Inventory.TotalArmor;
    public byte SecureMode { get; private set; }
    public float CarryStrength => TotalCapacity - Inventory.TotalWeight;
    public override bool UsingDistanceWeapon => Inventory.Weapon is IDistanceWeapon;
    public bool Recovering { get; private set; }
    public override bool CanSeeInvisible => FlagIsEnabled(PlayerFlag.CanSeeInvisibility);
    public override bool CanBeSeen => FlagIsEnabled(PlayerFlag.CanBeSeen);

    public ushort GetSkillLevel(SkillType skillType)
    {
        var hasSkill = Skills.TryGetValue(skillType, out var skill);
        var skillLevel = hasSkill ? skill.Level : 1;
        var skillBonus = skill?.Bonus ?? 0;
        var totalSkill = skillLevel + skillBonus;
        return (ushort)Math.Max(0, totalSkill);
    }

    public byte GetSkillTries(SkillType skillType)
    {
        return (byte)(Skills.TryGetValue(skillType, out var skill) ? skill.Count : 0);
    }

    public sbyte GetSkillBonus(SkillType skill)
    {
        return Skills[skill].Bonus;
    }

    public void AddSkillBonus(SkillType skillType, sbyte increase)
    {
        if (increase == 0) return;
        if (Skills is null) return;
        if (!Skills.TryGetValue(skillType, out _))
            Skills.Add(skillType, new Skill(skillType, 1, 1)); //todo: review those skill values

        Skills[skillType]?.AddBonus(increase);
        OnAddedSkillBonus?.Invoke(this, skillType, increase);
    }

    public void RemoveSkillBonus(SkillType skillType, sbyte decrease)
    {
        if (decrease == 0) return;

        Skills[skillType]?.RemoveBonus(decrease);
        OnRemovedSkillBonus?.Invoke(this, skillType, decrease);
    }

    public byte GetSkillPercent(SkillType skill)
    {
        var rate = Vocations.Vocation.DefaultSkillMultiplier;
        Vocation.Skills?.TryGetValue(skill, out rate);
        return (byte)Skills[skill].GetPercentage(rate);
    }

    public bool KnowsCreatureWithId(uint creatureId)
    {
        return KnownCreatures.ContainsKey(creatureId);
    }

    public void AddKnownCreature(uint creatureId)
    {
        KnownCreatures.TryAdd(creatureId, DateTime.Now.Ticks);
    }

    public uint ChooseToRemoveFromKnownSet()
    {
        // if the buffer is full we need to choose a vitim.
        while (KnownCreatures.Count == KNOWN_CREATURE_LIMIT)
            foreach (var candidate in
                     KnownCreatures.OrderBy(kvp => kvp.Value)
                         .ToList()) // .ToList() prevents modifiying an enumerating collection in the rare case we hit an exception down there.
                try
                {
                    if (KnownCreatures.Remove(candidate.Key)) return candidate.Key;
                }
                catch
                {
                    // happens when 2 try to remove time, which we don't care too much.
                }

        return uint.MinValue; // 0
    }

    public override void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators)
    {
        TogglePacifiedCondition(fromTile, toTile);
        Containers.CloseDistantContainers();
        base.OnMoved(fromTile, toTile, spectators);
    }

    public override bool CanSee(ICreature otherCreature)
    {
        return !otherCreature.IsInvisible ||
               otherCreature is IPlayer && otherCreature.CanBeSeen ||
               CanSeeInvisible;
    }

    public override void TurnInvisible()
    {
        SetTemporaryOutfit(0, 0, 0, 0, 0, 0, 0);
        base.TurnInvisible();
    }

    public override void TurnVisible()
    {
        BackToOldOutfit();
        base.TurnVisible();
    }

    public override void SetAsEnemy(ICreature creature)
    {
        if (creature is not IMonster) return;
        SetAsInFight();
    }

    public void StopShopping()
    {
        TradingWithNpc?.StopSellingToCustomer(this);
        TradingWithNpc = null;
    }

    public void StartShopping(IShopperNpc npc)
    {
        TradingWithNpc = npc;
    }

    public void ChangeFightMode(FightMode mode)
    {
        FightMode = mode;
    }

    public void ChangeChaseMode(ChaseMode mode)
    {
        var oldChaseMode = ChaseMode;
        ChaseMode = mode;

        if (ChaseMode == ChaseMode.Follow && AutoAttackTarget is not null)
        {
            Follow(AutoAttackTarget as IWalkableCreature, PathSearchParams);
            return;
        }

        StopFollowing();

        OnChangedChaseMode?.Invoke(this, oldChaseMode, mode);
    }

    public void ChangeSecureMode(byte mode)
    {
        SecureMode = mode;
    }

    public override int ShieldDefend(int attack)
    {
        var defense = Inventory.TotalDefense * Skills[SkillType.Shielding].Level *
            (DefenseFactor / 100d) - attack / 100d * ArmorRating * (Vocation.Formula?.Defense ?? 1f);

        var resultDamage = (int)(attack - defense);
        if (resultDamage <= 0) IncreaseSkillCounter(SkillType.Shielding, 1);
        return resultDamage;
    }

    public override int ArmorDefend(int damage)
    {
        switch (ArmorRating)
        {
            case > 3:
            {
                var min = ArmorRating / 2 * (Vocation.Formula?.Armor ?? 1f);
                var max = (ArmorRating / 2 * 2 - 1) * (Vocation.Formula?.Armor ?? 1f);
                damage -= (ushort)GameRandom.Random.NextInRange(min, max);
                break;
            }
            case > 0:
                --damage;
                break;
        }

        return damage;
    }

    public void SendMessageTo(ISociableCreature to, SpeechType speechType, string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        OnSentMessage?.Invoke(this, to, speechType, message);
    }

    public virtual bool CastSpell(string message)
    {
        if (!SpellList.TryGet(message.Trim(), out var spell)) return false;
        if (!spell.Invoke(this, message, out var error))
        {
            OnCannotUseSpell?.Invoke(this, spell, error);
            return true;
        }

        var talkType = SpeechType.MonsterSay;

        Cooldowns.Start(CooldownType.Spell, 1000); //todo: 1000 should be a const

        if (spell.IncreaseSkill) IncreaseSkillCounter(SkillType.Magic, spell.Mana);

        if (!spell.ShouldSay) return true;

        base.Say(message, talkType);

        return true;
    }

    public bool HasEnoughMana(ushort mana)
    {
        return Mana >= mana;
    }

    public void ConsumeMana(ushort mana)
    {
        if (mana == 0) return;
        if (!HasEnoughMana(mana)) return;

        Mana -= mana;
        OnStatusChanged?.Invoke(this);
    }

    public bool HasEnoughLevel(ushort level)
    {
        return Level >= level;
    }

    public void LookAt(ITile tile)
    {
        var isClose = Location.IsNextTo(tile.Location);
        if (tile.TopCreatureOnStack is null && tile.TopItemOnStack is null) return;

        IThing thing = tile.TopCreatureOnStack is null ? tile.TopItemOnStack : tile.TopCreatureOnStack;
        OnLookedAt?.Invoke(this, thing, isClose);
    }

    public void LookAt(byte containerId, sbyte containerSlot)
    {
        if (Containers[containerId][containerSlot] is not IThing thing) return;
        OnLookedAt?.Invoke(this, thing, true);
    }

    public void LookAt(Slot slot)
    {
        if (Inventory[slot] is not IThing thing) return;
        OnLookedAt?.Invoke(this, thing, true);
    }

    public void Read(IReadable readable)
    {
        OnReadText?.Invoke(this, readable, readable.Text);
    }

    public void Write(IReadable readable, string text)
    {
        var result = readable.Write(text, this);
        if (result.Failed)
        {
            OperationFailService.Display(CreatureId, TextConstants.NOT_POSSIBLE);
            return;
        }

        OnWroteText?.Invoke(this, readable, readable.Text);
    }

    public bool Logout(bool forced = false)
    {
        if (CannotLogout && forced == false)
        {
            OperationFailService.Display(CreatureId, "You may not logout during or immediately after a fight");
            return false;
        }

        StopAttack();
        StopFollowing();
        StopWalking();
        Containers.CloseAll();
        ChangeOnlineStatus(false);
        PlayerParty.LeaveParty();
        PlayerParty.RejectAllInvites();

        OnLoggedOut?.Invoke(this);
        return true;
    }

    public bool Login()
    {
        StopAttack();
        StopFollowing();
        StopWalking();
        ChangeOnlineStatus(true);

        KnownCreatures.Clear();
        OnLoggedIn?.Invoke(this);

        return true;
    }

    public void HealMana(ushort increasing)
    {
        if (increasing <= 0) return;

        if (Mana == MaxMana) return;

        Mana = Mana + increasing >= MaxMana ? MaxMana : (ushort)(Mana + increasing);
        OnStatusChanged?.Invoke(this);
    }

    public void Recover()
    {
        if (Cooldowns.Expired(CooldownType.HealthRecovery)) Heal(Vocation.GainHpAmount, this);
        if (Cooldowns.Expired(CooldownType.ManaRecovery)) HealMana(Vocation.GainManaAmount);
        if (Cooldowns.Expired(CooldownType.SoulRecovery)) HealSoul(1);

        //todo: start these cooldowns when player logs in
        Cooldowns.Start(CooldownType.HealthRecovery, Vocation.GainHpTicks * 1000);
        Cooldowns.Start(CooldownType.ManaRecovery, Vocation.GainManaTicks * 1000);
        Cooldowns.Start(CooldownType.SoulRecovery, Vocation.GainSoulTicks * 1000);
    }

    public void Use(IUsable item)
    {
        if(!item.IsCloseTo(this)) return;

        item.Use(this);
    }

    public void Use(IUsableOn item, ICreature onCreature)
    {
        if (!Cooldowns.Expired(CooldownType.UseItem))
        {
            OnExhausted?.Invoke(this);
            return;
        }

        if(!item.IsCloseTo(this)) return;

        if (item is IEquipmentRequirement requirement && !requirement.CanBeUsed(this))
        {
            OperationFailService.Display(CreatureId, requirement.ValidationError);
            return;
        }

        var result = false;

        if (onCreature is ICombatActor enemy)
        {
            switch (item)
            {
                case IUsableAttackOnCreature useableAttackOnCreature:
                    result = Attack(enemy, useableAttackOnCreature);
                    break;
                case IUsableOnCreature useableOnCreature:
                    useableOnCreature.Use(this, onCreature);
                    result = true;
                    break;
                case IUsableOnTile useableOnTile:
                    result = useableOnTile.Use(this, onCreature.Tile);
                    break;
            }
        }

        if (result) OnUsedItem?.Invoke(this, onCreature, item);
        Cooldowns.Start(CooldownType.UseItem, item.CooldownTime);
    }

    public void Use(IUsableOn item, IItem onItem)
    {
        if (!Cooldowns.Expired(CooldownType.UseItem))
        {
            OnExhausted?.Invoke(this);
            return;
        }

        if(!item.IsCloseTo(this)) return;

        if (item is IEquipmentRequirement requirement && !requirement.CanBeUsed(this))
        {
            OperationFailService.Display(CreatureId, requirement.ValidationError);
            return;
        }

        if (item is not IUsableOnItem useableOnItem) return;

        useableOnItem.Use(this, onItem);
        OnUsedItem?.Invoke(this, onItem, item);
        Cooldowns.Start(CooldownType.UseItem, 1000);
    }

    public Result Use(IUsableOn item, ITile targetTile)
    {
        if (!Cooldowns.Expired(CooldownType.UseItem))
        {
            OnExhausted?.Invoke(this);
            return Result.NotPossible;
        }

        if(!item.IsCloseTo(this)) return Result.NotPossible;
        
        if (item is IEquipmentRequirement requirement && !requirement.CanBeUsed(this))
        {
            OperationFailService.Display(CreatureId, requirement.ValidationError);
            return Result.NotPossible;
        }

        if (targetTile.TopItemOnStack is not { } onItem) return Result.NotPossible;

        var result = item switch
        {
            IUsableAttackOnTile useableAttackOnTile => Attack(targetTile, useableAttackOnTile),
            IUsableOnTile useableOnTile => useableOnTile.Use(this, targetTile),
            IUsableOnItem useableOnItem => useableOnItem.Use(this, onItem),
            _ => false
        };

        if (result) OnUsedItem?.Invoke(this, onItem, item);
        Cooldowns.Start(CooldownType.UseItem, 1000);

        return Result.Success;
    }

    public bool Feed(IFood food)
    {
        if (food is null) return false;

        var regenerationMs = (uint)food.Duration * 1000;
        var maxRegenerationTime = (uint)1200 * 1000;

        if (Conditions.TryGetValue(ConditionType.Regeneration, out var condition))
        {
            if (condition.RemainingTime + regenerationMs >=
                maxRegenerationTime) //todo: this number should be configurable
            {
                OperationFailService.Display(CreatureId, "You are full");
                return false;
            }

            condition.Extend(regenerationMs, maxRegenerationTime);
        }
        else
        {
            AddCondition(new Condition(ConditionType.Regeneration, regenerationMs, OnHungry));
        }

        Recovering = true;
        return true;
    }

    public Result<OperationResult<IItem>> PickItemFromGround(IItem item, ITile tile, byte amount = 1) =>
        PlayerHand.PickItemFromGround(item, tile, amount);

    public Result<OperationResult<IItem>> MoveItem(IItem item,IHasItem source, IHasItem destination, byte amount, byte fromPosition,
        byte? toPosition) =>
        PlayerHand.Move(item, source, destination, amount, fromPosition, toPosition);

    public override void SetAttackTarget(ICreature target)
    {
        base.SetAttackTarget(target);
        if (target.CreatureId != 0 && ChaseMode == ChaseMode.Follow) Follow(target, PathSearchParams);
    }

    public void Hear(ICreature from, SpeechType speechType, string message)
    {
        if (from is null || speechType == SpeechType.None || string.IsNullOrWhiteSpace(message)) return;

        OnHear?.Invoke(from, this, speechType, message);
    }

    public bool Sell(IItemType item, byte amount, bool ignoreEquipped)
    {
        if (!ignoreEquipped) return true;
        if (Inventory.BackpackSlot?.Map is null) return false;
        if (!Inventory.BackpackSlot.Map.TryGetValue(item.TypeId, out var itemTotalAmount)) return false;

        if (itemTotalAmount < amount) return false;

        Inventory.BackpackSlot.RemoveItem(item, amount);

        TradingWithNpc.BuyFromCustomer(this, item, amount);

        return true;
    }

    public void ReceivePayment(IEnumerable<IItem> coins, ulong total)
    {
        if (CanReceiveInCashPayment(coins))
            foreach (var coin in coins)
                Inventory.BackpackSlot.AddItem(coin, true);
        else
            BankAmount += total;
    }

    public virtual void WithdrawFromBank(ulong amount)
    {
        if (BankAmount >= amount) BankAmount -= amount;
    }

    public bool CanReceiveInCashPayment(IEnumerable<IItem> coins)
    {
        var totalWeight = coins.Sum(x => x is ICumulative cumulative ? cumulative.Weight : 0);
        var totalFreeSlots = Inventory.BackpackSlot?.TotalOfFreeSlots ?? 0;

        return !(totalWeight > CarryStrength) && totalFreeSlots >= coins.Count();
    }

    public void ReceivePurchasedItems(INpc from, SaleContract saleContract, params IItem[] items)
    {
        if (items is null) return;

        var possibleAmountOnInventory = saleContract.PossibleAmountOnInventory;

        foreach (var item in items)
        {
            if (item is null) continue;

            if (possibleAmountOnInventory > 0)
            {
                possibleAmountOnInventory = (uint)Math.Max(0, (int)possibleAmountOnInventory - item.Amount);
                var result = Inventory.AddItem(item);
                if (result.Succeeded)
                {
                    if (!result.Value.HasAnyOperation) continue;
                    if (result.Value.Operations[0].Item2 != Operation.Removed) continue;
                }
            }

            Inventory.BackpackSlot?.AddItem(item, true);
        }
    }


    public void ResetIdleTime()
    {
        _idleTime = 0;
    }

    public bool CanMoveThing(Location location)
    {
        return Location.GetSqmDistance(location) <= MapConstants.MAX_DISTANCE_MOVE_THING;
    }

    public void OnLevelAdvance(SkillType type, int fromLevel, int toLevel)
    {
        if (type == SkillType.Level)
        {
            var levelDiff = toLevel - fromLevel;
            MaxHealthPoints += (uint)(levelDiff * Vocation.GainHp);
            MaxMana += (ushort)(levelDiff * Vocation.GainMana);
            TotalCapacity += (uint)(levelDiff * Vocation.GainCap);
            ResetHealthPoints();
            ResetMana();
            ChangeSpeed(LevelBasesSpeed);
        }

        OnLevelAdvanced?.Invoke(this, type, fromLevel, toLevel);
    }

    public virtual void SetFlags(params PlayerFlag[] flags)
    {
        foreach (var flag in flags) _flags |= (ulong)flag;
    }

    public void ResetMana()
    {
        HealMana(MaxMana);
    }

    public void IncreaseSkillCounter(SkillType skill, uint value)
    {
        if (!Skills.ContainsKey(skill)) return;

        var rate = Vocations.Vocation.DefaultSkillMultiplier;

        Vocation?.Skills?.TryGetValue(skill, out rate);

        Skills[skill].IncreaseCounter(value, rate);
    }

    public override bool HasImmunity(Immunity immunity)
    {
        return false;
        //todo: add immunity check
    }

    public void SetAsInFight()
    {
        if (IsPacified) return;

        if (HasCondition(ConditionType.InFight, out var condition))
        {
            condition.Start(this);
            return;
        }

        AddCondition(new Condition(ConditionType.InFight, 60000));
    }

    private void TogglePacifiedCondition(IDynamicTile fromTile, IDynamicTile toTile)
    {
        switch (fromTile.ProtectionZone)
        {
            case false when toTile.ProtectionZone is true:
                RemoveCondition(ConditionType.InFight);
                AddCondition(new Condition(ConditionType.Pacified, 0));
                break;
            case true when toTile.ProtectionZone is false:
                RemoveCondition(ConditionType.Pacified);
                break;
        }
    }

    public override bool TryWalkTo(params Direction[] directions)
    {
        ResetIdleTime();
        return base.TryWalkTo(directions);
    }

    public override bool OnAttack(ICombatActor enemy, out CombatAttackType combat)
    {
        combat = new CombatAttackType();

        var canUse = true;

        if (Inventory.IsUsingWeapon) canUse = Inventory.Weapon.Use(this, enemy, out combat);

        if (canUse) IncreaseSkillCounter(SkillInUse, 1);

        return canUse;
    }

    public override CombatDamage OnImmunityDefense(CombatDamage damage)
    {
        if (!HasImmunity(damage.Type.ToImmunity())) return damage;
        damage.SetNewDamage(0);
        return damage;
    }

    public void ChangeOnlineStatus(bool online)
    {
        OnChangedOnlineStatus?.Invoke(this, online);
    }

    public override bool CanBlock(DamageType damage)
    {
        return Inventory.HasShield && base.CanBlock(damage);
    }

    public void HealSoul(ushort increasing)
    {
        if (increasing <= 0) return;

        if (SoulPoints == MaxSoulPoints) return;

        SoulPoints = SoulPoints + increasing >= MaxSoulPoints ? MaxSoulPoints : (byte)(SoulPoints + increasing);
        OnStatusChanged?.Invoke(this);
    }

    public override void OnDamage(IThing enemy, CombatDamage damage)
    {
        if (damage.Type == DamageType.ManaDrain) ConsumeMana(damage.Damage);
        else
            ReduceHealth(damage);
    }

    public void OnHungry()
    {
        Recovering = false;
    }

    public override ILoot DropLoot()
    {
        return null;
    }

    public void StopAllActions()
    {
        StopWalking();
        StopAttack();
        StopFollowing();
    }

    #region Guild

    private string GuildText => HasGuild && Guild is { } guid ? $". He is member of the {guid.Name}" : string.Empty;
    public ushort GuildLevel { get; set; }
    public bool HasGuild => Guild is { };
    public IGuild Guild { get; init; }

    #endregion

    #region Flags

    public void UnsetFlag(PlayerFlag flag)
    {
        _flags &= ~(ulong)flag;
    }

    public void SetFlag(PlayerFlag flag)
    {
        _flags |= (ulong)flag;
    }

    public bool FlagIsEnabled(PlayerFlag flag)
    {
        return (_flags & (ulong)flag) != 0;
    }

    #endregion

    #region Events

    public event PlayerLevelAdvance OnLevelAdvanced;
    public event PlayerGainSkillPoint OnGainedSkillPoint;
    public event ReduceMana OnStatusChanged;
    public event CannotUseSpell OnCannotUseSpell;
    public event LookAt OnLookedAt;
    public event UseSpell OnUsedSpell;
    public event UseItem OnUsedItem;
    public event LogIn OnLoggedIn;
    public event LogOut OnLoggedOut;
    public event ChangeOnlineStatus OnChangedOnlineStatus;
    public event SendMessageTo OnSentMessage;

    public event Exhaust OnExhausted;
    public event Hear OnHear;
    public event ChangeChaseMode OnChangedChaseMode;
    public event AddSkillBonus OnAddedSkillBonus;
    public event RemoveSkillBonus OnRemovedSkillBonus;
    public event ReadText OnReadText;
    public event WroteText OnWroteText;

    #endregion
}