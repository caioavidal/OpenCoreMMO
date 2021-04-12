using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Conditions;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Common.Texts;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Game.Creatures.Vocations;
using NeoServer.Game.DataStore;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Server.Model.Players
{
    public class Player : CombatActor, IPlayer
    {
        public Player(uint id, string characterName, ChaseMode chaseMode, uint capacity, ushort healthPoints, ushort maxHealthPoints, byte vocation,
            Gender gender, bool online, ushort mana, ushort maxMana, FightMode fightMode, byte soulPoints, byte soulMax, IDictionary<SkillType, ISkill> skills, ushort staminaMinutes,
            IOutfit outfit, IDictionary<Slot, Tuple<IPickupable, ushort>> inventory, ushort speed,
            Location location)
             : base(new CreatureType(characterName, string.Empty, maxHealthPoints, speed, new Dictionary<LookType, ushort> { { LookType.Corpse, 3058 } }), outfit, healthPoints)
        {
            Id = id;
            CharacterName = characterName;
            ChaseMode = chaseMode;
            TotalCapacity = capacity;
            Inventory = new PlayerInventory(this, inventory);
            VocationType = vocation;
            Gender = gender;
            Online = online;
            Mana = mana;
            MaxMana = maxMana;
            FightMode = fightMode;
            MaxSoulPoints = soulMax;
            SoulPoints = soulPoints;
            Skills = skills;
            StaminaMinutes = staminaMinutes;
            Outfit = outfit;
            Speed = speed == 0 ? LevelBasesSpeed : speed;

            Location = location;

            Containers = new PlayerContainerList(this);

            KnownCreatures = new Dictionary<uint, long>();//todo

            foreach (var skill in Skills.Values)
            {
                skill.OnAdvance += OnLevelAdvance;
                skill.OnIncreaseSkillPoints += (skill) => OnGainedSkillPoint?.Invoke(this, skill);
            }
        }

        public event PlayerLevelAdvance OnLevelAdvanced;
        public event PlayerGainSkillPoint OnGainedSkillPoint;

        public event CancelWalk OnCancelledWalk;

        public event ReduceMana OnStatusChanged;
        public event CannotUseSpell OnCannotUseSpell;
        public event LookAt OnLookedAt;
        public event UseSpell OnUsedSpell;
        public event UseItem OnUsedItem;
        public event LogIn OnLoggedIn;
        public event LogOut OnLoggedOut;
        public event PlayerJoinChannel OnJoinedChannel;
        public event PlayerExitChannel OnExitedChannel;
        public override event DropLoot OnDropLoot;
        public event AddToVipList OnAddedToVipList;
        public event PlayerLoadVipList OnLoadedVipList;
        public event ChangeOnlineStatus OnChangedOnlineStatus;
        public event SendMessageTo OnSentMessage;
        public event InviteToParty OnInviteToParty;
        public event InviteToParty OnInvitedToParty;
        public event RevokePartyInvite OnRevokePartyInvite;
        public event RejectPartyInvite OnRejectedPartyInvite;
        public event JoinParty OnJoinedParty;
        public event LeaveParty OnLeftParty;
        public event PassPartyLeadership OnPassedPartyLeadership;

        public event Hear OnHear;

        public ushort GuildId { get; init; }
        public bool HasGuild => GuildId > 0;
        public IGuild Guild => GuildStore.Data.Get(GuildId);
        public ulong BankAmount { get; private set; }
        public ulong TotalMoney => BankAmount + Inventory.TotalMoney;
        public IParty Party { get; private set; }
        private IParty PartyInvite;
        private bool IsPartyLeader => Party?.IsLeader(this) ?? false;

        public void LoadBank(ulong amount) => BankAmount = amount;
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

        private ulong flags = 0b000000000000000000000;

        public void UnsetFlag(PlayerFlag flag) => flags &= ~(ulong)flag;

        public void SetFlag(PlayerFlag flag) => flags |= (ulong)flag;
        public virtual void SetFlags(params PlayerFlag[] flags)
        {
            foreach (var flag in flags)
            {
                this.flags |= (ulong)flag;
            }
        }

        public void LoadVipList(IEnumerable<(uint, string)> vips)
        {
            var vipList = new HashSet<(uint, string)>();
            foreach (var vip in vips)
            {
                if (string.IsNullOrWhiteSpace(vip.Item2)) continue;

                VipList.Add(vip.Item1);
                vipList.Add(vip);
            }

            OnLoadedVipList?.Invoke(this, vipList);
        }
        public bool FlagIsEnabled(PlayerFlag flag) => (flags & (ulong)flag) != 0;
        private ushort LevelBasesSpeed => (ushort)(220 + (2 * (Level - 1)));

        private uint IdleTime;
        public uint AccountId { get; init; }
        public string CharacterName { get; private set; }
        public override IOutfit Outfit { get; protected set; }
        public IDictionary<SkillType, ISkill> Skills { get; private set; }
        public IPlayerContainerList Containers { get; }
        public bool HasDepotOpened => Containers.HasAnyDepotOpened;
        public Dictionary<uint, long> KnownCreatures { get; }
        public IShopperNpc TradingWithNpc { get; private set; }
        public IVocation Vocation => VocationStore.TryGetValue(VocationType, out var vocation) ? vocation : null;
        public ChaseMode ChaseMode { get; private set; }
        public uint TotalCapacity { get; private set; }
        public ushort Level => Skills[SkillType.Level].Level;
        public byte VocationType { get; private set; }
        public Gender Gender { get; private set; }
        public bool Online { get; private set; }
        public ushort Mana { get; private set; }
        public ushort MaxMana { get; private set; }
        public HashSet<uint> VipList { get; set; } = new HashSet<uint>();
        public FightMode FightMode { get; private set; }
     
        public bool Shopping => TradingWithNpc is not null;
        public IEnumerable<IChatChannel> PrivateChannels
        {
            get
            {
                if (HasGuild) yield return Guild.Channel;
                if (Party?.Channel is not null) yield return Party.Channel;
            }
        }
        private IDictionary<ushort, IChatChannel> personalChannels;
        private byte soulPoints;
        public byte SoulPoints
        {
            get => soulPoints;
            private set => soulPoints = value > MaxSoulPoints ? MaxSoulPoints : value;
        }

        public byte MaxSoulPoints { get; private set; }
        public IInventory Inventory { get; set; }

        public ushort StaminaMinutes { get; private set; }

        public uint Experience
        {
            get
            {
                if (Skills.TryGetValue(SkillType.Level, out ISkill skill))
                {
                    return (uint)skill.Count;
                }
                return 0;
            }
        }
        public IEnumerable<IChatChannel> PersonalChannels => personalChannels?.Values;
        public void AddPersonalChannel(IChatChannel channel)
        {
            personalChannels = personalChannels ?? new Dictionary<ushort, IChatChannel>();
            personalChannels.Add(channel.Id, channel);
        }
        public void ResetMana() => HealMana(MaxMana);
        public byte LevelPercent => GetSkillPercent(SkillType.Level);
        public void IncreaseSkillCounter(SkillType skill, uint value)
        {
            if (!Skills.ContainsKey(skill)) return;

            Skills[skill].IncreaseCounter(value);
        }

        public void ResetIdleTime() => IdleTime = 0;

        public override void GainExperience(uint exp)
        {
            if (exp == 0)
            {
                return;
            }

            IncreaseSkillCounter(SkillType.Level, exp);
            base.GainExperience(exp);
        }

        public bool CannotLogout => !(Tile?.ProtectionZone ?? false) && InFight;
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

        public SkillType SkillInUse
        {
            get
            {
                if (Inventory.Weapon is IWeapon weapon)
                {
                    return weapon.Type switch
                    {
                        WeaponType.Club => SkillType.Club,
                        WeaponType.Sword => SkillType.Sword,
                        WeaponType.Axe => SkillType.Axe,
                        WeaponType.Ammunition => SkillType.Distance,
                        WeaponType.Distance => SkillType.Distance,
                        _ => SkillType.Fist
                    };
                }
                return SkillType.Fist;
            }
        }

        public ushort CalculateAttackPower(float attackRate, ushort attack) => (ushort)(attackRate * DamageFactor * attack * Skills[SkillInUse].Level + (Level / 5));

        public uint Id { get; }
        public override ushort MinimumAttackPower => (ushort)(Level / 5);

        public override ushort ArmorRating => Inventory.TotalArmor;
        public byte SecureMode { get; private set; }
        public float CarryStrength => TotalCapacity - Inventory.TotalWeight;
        public bool IsPacified => Conditions.ContainsKey(ConditionType.Pacified);
        public override bool UsingDistanceWeapon => Inventory.Weapon is IDistanceWeaponItem;
        public bool Recovering { get; private set; }
        public ushort GuildLevel { get; set; }

        public override bool CanSeeInvisible => FlagIsEnabled(PlayerFlag.CanSeeInvisibility);

        public override bool CanBeSeen => FlagIsEnabled(PlayerFlag.CanBeSeen);

        public bool IsInParty => Party is not null;

        public byte GetSkillInfo(SkillType skill) => (byte)Skills[skill].Level;
        public byte GetSkillPercent(SkillType skill) => (byte)Skills[skill].Percentage;
        public bool KnowsCreatureWithId(uint creatureId) => KnownCreatures.ContainsKey(creatureId);
        public bool CanMoveThing(Location location) => Location.GetSqmDistance(location) <= MapConstants.MAX_DISTANCE_MOVE_THING;

        public void AddKnownCreature(uint creatureId) => KnownCreatures.TryAdd(creatureId, DateTime.Now.Ticks);
        const int KnownCreatureLimit = 250; //todo: for version 8.60

        public uint ChooseToRemoveFromKnownSet()
        {
            // if the buffer is full we need to choose a vitim.
            while (KnownCreatures.Count == KnownCreatureLimit)
            {
                foreach (var candidate in KnownCreatures.OrderBy(kvp => kvp.Value).ToList()) // .ToList() prevents modifiying an enumerating collection in the rare case we hit an exception down there.
                {
                    try
                    {
                        if (KnownCreatures.Remove(candidate.Key))
                        {
                            return candidate.Key;
                        }
                    }
                    catch
                    {
                        // happens when 2 try to remove time, which we don't care too much.
                    }
                }
            }

            return uint.MinValue; // 0
        }

        public void ChangeOutfit(IOutfit outfit) => Outfit = outfit;

        public override void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators)
        {
            TogglePacifiedCondition(fromTile, toTile);
            Containers.CloseDistantContainers();
            base.OnMoved(fromTile, toTile, spectators);
        }
        public override bool CanSee(ICreature otherCreature)
        {
            return !otherCreature.IsInvisible || (otherCreature is IPlayer && otherCreature.CanBeSeen) || (CanSeeInvisible);
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

        public void StopShopping()
        {
            TradingWithNpc?.StopSellingToCustomer(this);
            TradingWithNpc = null;
        }
        public void StartShopping(IShopperNpc npc)
        {
            TradingWithNpc = npc;
        }
        private void TogglePacifiedCondition(IDynamicTile fromTile, IDynamicTile toTile)
        {
            if (fromTile.ProtectionZone is false && toTile.ProtectionZone is true)
            {
                RemoveCondition(ConditionType.InFight);
                AddCondition(new Condition(ConditionType.Pacified, 0));
            }

            if (fromTile.ProtectionZone is true && toTile.ProtectionZone is false) RemoveCondition(ConditionType.Pacified);
        }

        public override bool TryWalkTo(params Direction[] directions)
        {
            ResetIdleTime();
            return base.TryWalkTo(directions);
        }

        public void SetFightMode(FightMode mode)
        {
            FightMode = mode;
        }
        public void SetChaseMode(ChaseMode mode)
        {
            ChaseMode = mode;
            FollowModeEnabled = mode == ChaseMode.Follow;
            if (FollowModeEnabled && AutoAttackTarget is not null)
            {
                Follow(AutoAttackTarget as IWalkableCreature, PathSearchParams);
                return;
            }

            StopFollowing();
        }

        public void SetSecureMode(byte mode) => SecureMode = mode;

        public void CancelWalk()
        {
            StopWalking();
            OnCancelledWalk(this);
        }

        public override int ShieldDefend(int attack)
        {
            var resultDamage = (int)(attack - Inventory.TotalDefense * Skills[SkillType.Shielding].Level * (DefenseFactor / 100d) - (attack / 100d) * ArmorRating);
            if (resultDamage <= 0)
            {
                IncreaseSkillCounter(SkillType.Shielding, 1);
            }
            return resultDamage;
        }

        public override int ArmorDefend(int damage)
        {
            if (ArmorRating > 3)
            {
                var min = ArmorRating / 2;
                var max = (ArmorRating / 2) * 2 - 1;
                damage -= (ushort)GameRandom.Random.NextInRange(min, max);
            }
            else if (ArmorRating > 0)
            {
                --damage;
            }
            return damage;
        }
        public override bool OnAttack(ICombatActor enemy, out CombatAttackType combat)
        {
            combat = new CombatAttackType();
            var canUse = Inventory.Weapon?.Use(this, enemy, out combat) ?? false;

            if (canUse)
            {
                IncreaseSkillCounter(SkillInUse, 1);
            }

            return canUse;
        }
        public void SendMessageTo(ISociableCreature to, SpeechType speechType, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            OnSentMessage?.Invoke(this, to, speechType, message);
        }

        public virtual bool CastSpell(string message)
        {
            if (SpellList.TryGet(message.Trim(), out var spell))
            {
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

            return false;
        }
        public override void Say(string message, SpeechType talkType, ICreature receiver = null)
        {
            base.Say(message, talkType, receiver);
        }

        public bool HasEnoughMana(ushort mana) => Mana >= mana;
        public void ConsumeMana(ushort mana)
        {
            if (mana == 0) return;
            if (!HasEnoughMana(mana)) return;

            Mana -= mana;
            OnStatusChanged?.Invoke(this);
        }
        public bool HasEnoughLevel(ushort level) => Level >= level;

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

        public override CombatDamage OnImmunityDefense(CombatDamage damage)
        {
            return damage; //todo
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
            ChangeOnlineStatus(online: false);
            LeaveParty();
            RejectInvite();

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

        public void ChangeOnlineStatus(bool online) => OnChangedOnlineStatus?.Invoke(this, online);

        public override bool CanBlock(DamageType damage)
        {
            if (!Inventory.HasShield) return false;
            return base.CanBlock(damage);
        }
        public void HealMana(ushort increasing)
        {
            if (increasing <= 0) return;

            if (Mana == MaxMana) return;

            Mana = Mana + increasing >= MaxMana ? MaxMana : (ushort)(Mana + increasing);
            OnStatusChanged?.Invoke(this);
        }
        public void HealSoul(ushort increasing)
        {
            if (increasing <= 0) return;

            if (SoulPoints == MaxSoulPoints) return;

            SoulPoints = SoulPoints + increasing >= MaxSoulPoints ? MaxSoulPoints : (byte)(SoulPoints + increasing);
            OnStatusChanged?.Invoke(this);
        }

        public void Recover()
        {
            if (Cooldowns.Expired(CooldownType.HealthRecovery)) Heal(Vocation.GainHpAmount);
            if (Cooldowns.Expired(CooldownType.ManaRecovery)) HealMana(Vocation.GainManaAmount);
            if (Cooldowns.Expired(CooldownType.SoulRecovery)) HealSoul(1);

            //todo: start these cooldowns when player logs in
            Cooldowns.Start(CooldownType.HealthRecovery, Vocation.GainHpTicks * 1000);
            Cooldowns.Start(CooldownType.ManaRecovery, Vocation.GainManaTicks * 1000);
            Cooldowns.Start(CooldownType.SoulRecovery, Vocation.GainSoulTicks * 1000);
        }

        public override void OnDamage(IThing enemy, CombatDamage damage)
        {
            if (damage.Type == DamageType.ManaDrain) ConsumeMana(damage.Damage);
            else
                ReduceHealth(damage);
        }
        public void Use(IUseable item)
        {
            item.Use(this);
        }
        public void Use(IUseableOn item, ICreature onCreature)
        {
            if (item is IItemRequirement requirement && !requirement.CanBeUsed(this))
            {
                OperationFailService.Display(CreatureId, requirement.ValidationError);
                return;
            }
            var result = false;

            if (onCreature is ICombatActor enemy)
            {
                if (item is IUseableAttackOnCreature useableAttackOnCreature) result = Attack(enemy, useableAttackOnCreature);
                else if (item is IUseableOnCreature useableOnCreature) { useableOnCreature.Use(this, onCreature); result = true; }
                else if (item is IUseableOnTile useableOnTile) result = useableOnTile.Use(this, onCreature.Tile);
            }

            if (result) OnUsedItem?.Invoke(this, onCreature, item);
        }
        public void Use(IUseableOn item, IItem onItem)
        {
            if (item is IItemRequirement requirement && !requirement.CanBeUsed(this))
            {
                OperationFailService.Display(CreatureId, requirement.ValidationError);
                return;
            }

            if (item is not IUseableOnItem useableOnItem) return;

            useableOnItem.Use(this, onItem);
            OnUsedItem?.Invoke(this, onItem, item);
        }
        public void Use(IUseableOn item, ITile onTile)
        {
            if (item is IItemRequirement requirement && !requirement.CanBeUsed(this))
            {
                OperationFailService.Display(CreatureId, requirement.ValidationError);
                return;
            }

            if (onTile.TopItemOnStack is not IItem onItem) return;

            var result = false;

            if (item is IUseableAttackOnTile useableAttackOnTile) result = Attack(onTile, useableAttackOnTile);
            else if (item is IUseableOnTile useableOnTile) result = useableOnTile.Use(this, onTile);
            else if (item is IUseableOnItem useableOnItem) result = useableOnItem.Use(this, onItem);

            if (result) OnUsedItem?.Invoke(this, onItem, item);
        }
        public bool Feed(IFood food)
        {
            if (food is null) return false;

            var regenerationMs = (uint)food.Duration * 1000;
            var maxRegenerationTime = (uint)1200 * 1000;

            if (Conditions.TryGetValue(ConditionType.Regeneration, out var condition))
            {
                if (condition.RemainingTime + regenerationMs >= maxRegenerationTime) //todo: this number should be configurable
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

        public void OnHungry() => Recovering = false;

        public Result MoveItem(IStore source, IStore destination, IItem thing, byte amount, byte fromPosition, byte? toPosition)
        {
            if (thing is not IPickupable) return Result.NotPossible;
            if (thing.Location.Type == LocationType.Ground && !Location.IsNextTo(thing.Location)) return new Result(InvalidOperation.TooFar);

            return source.SendTo(destination, thing, amount, fromPosition, toPosition).ResultValue;
        }

        public override void SetAttackTarget(ICreature target)
        {
            base.SetAttackTarget(target);
            if (target.CreatureId != 0 && ChaseMode == ChaseMode.Follow)
            {
                Follow(target, PathSearchParams);
            }
        }

        public bool CanEnterOnChannel(ushort channelId)
        {
            var channel = ChatChannelStore.Data.Get(channelId);
            return channel.PlayerCanJoin(this);
        }
        public bool JoinChannel(IChatChannel channel)
        {
            if (channel is null) return false;

            if (channel.HasUser(this))
            {
                OperationFailService.Display(CreatureId, "You've already joined this chat channel");
                return false;
            }
            if (!channel.AddUser(this))
            {
                OperationFailService.Display(CreatureId, "You cannot join this chat channel");
                return false;
            }

            OnJoinedChannel?.Invoke(this, channel);
            return true;
        }
        public bool ExitChannel(IChatChannel channel)
        {
            if (channel is null) return false;

            if (!channel.HasUser(this))
            {
                return false;
            }
            if (!channel.RemoveUser(this))
            {
                OperationFailService.Display(CreatureId, "You cannot exit this chat channel");
                return false;
            }

            OnExitedChannel?.Invoke(this, channel);
            return true;
        }
        public bool SendMessage(IChatChannel channel, string message)
        {
            if (!channel.WriteMessage(this, message, out var cancelMessage))
            {
                OperationFailService.Display(CreatureId, cancelMessage);
                return false;
            }
            return true;
        }
        public bool AddToVip(IPlayer player)
        {
            if (string.IsNullOrWhiteSpace(player.Name)) return false;

            if (VipList?.Count > 200)
            {
                OperationFailService.Display(CreatureId, "You cannot add more buddies.");
                return false;
            }
            if (player.FlagIsEnabled(PlayerFlag.SpecialVIP))
            {
                if (!FlagIsEnabled(PlayerFlag.SpecialVIP))
                {
                    OperationFailService.Display(CreatureId, TextConstants.CannotAddPlayerToVipList);
                    return false;
                }
            }
            if (!VipList.Add(player.Id))
            {
                OperationFailService.Display(CreatureId, "This player is already in your list.");
                return false;
            }

            OnAddedToVipList?.Invoke(this, player.Id, player.Name);
            return true;
        }
        public void RemoveFromVip(uint playerId)
        {
            VipList?.Remove(playerId);
        }
        public bool HasInVipList(uint playerId) => VipList.Contains(playerId);

        public void Hear(ICreature from, SpeechType speechType, string message)
        {
            if (from is null || speechType == SpeechType.None || string.IsNullOrWhiteSpace(message)) return;

            OnHear?.Invoke(from, this, speechType, message);
        }

        public bool Sell(IItemType item, byte amount, bool ignoreEquipped)
        {
            if (ignoreEquipped)
            {
                if (Inventory.BackpackSlot is null || Inventory.BackpackSlot.Map is null) return false;
                if (!Inventory.BackpackSlot.Map.TryGetValue(item.TypeId, out var itemTotalAmount)) return false;

                if (itemTotalAmount < amount) return false;

                Inventory.BackpackSlot.RemoveItem(item, amount);

                TradingWithNpc.BuyFromCustomer(this, item, amount);
            }

            return true;
        }
        public void ReceivePayment(IEnumerable<IItem> coins, ulong total)
        {
            if (CanReceiveInCashPayment(coins))
            {
                foreach (var coin in coins)
                {
                    Inventory.BackpackSlot.AddItem(coin, true);
                }
            }
            else
            {
                BankAmount += total;
            }
        }
        public virtual void WithdrawFromBank(ulong amount)
        {
            if (BankAmount >= amount) BankAmount = BankAmount - amount;
        }
        public bool CanReceiveInCashPayment(IEnumerable<IItem> coins)
        {
            var totalWeight = coins.Sum(x => x is ICumulative cumulative ? cumulative.Weight : 0);
            var totalFreeSlots = Inventory.BackpackSlot?.TotalFreeSlots ?? 0;

            if (totalWeight > CarryStrength || totalFreeSlots < coins.Count()) return false;

            return true;
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
                    if (result.IsSuccess)
                    {
                        if (!result.Value.HasAnyOperation) continue;
                        if (result.Value.Operations[0].Item2 != Operation.Removed) continue;
                    }
                }

                Inventory.BackpackSlot?.AddItem(item, true);
            }
        }

        public void InviteToParty(IPlayer invitedPlayer, IParty party)
        {
            if (invitedPlayer is null || invitedPlayer.CreatureId == CreatureId) return;

            if (invitedPlayer.IsInParty)
            {
                OperationFailService.Display(CreatureId, $"{invitedPlayer.Name} is already in a party");
                return;
            }

            var result = party.Invite(this, invitedPlayer);

            if (!result.IsSuccess)
            {
                OperationFailService.Display(CreatureId, TextConstants.OnlyLeadersCanInviteToParty);
                return;
            }

            var partyCreatedNow = Party is null;
            Party = party;
            OnInviteToParty?.Invoke(this, invitedPlayer, Party);

            if(partyCreatedNow) Party.OnPartyOver += PartyEmptyHandler;
        }

        public void ReceivePartyInvite(IPlayer leader, IParty party)
        {
            PartyInvite = party;
            OnInvitedToParty?.Invoke(leader, this, party);
            party.OnPartyOver += RejectInvite;
        }

        public void RejectInvite()
        {
            if (PartyInvite is null) return;
            PartyInvite.OnPartyOver -= RejectInvite;

            OnRejectedPartyInvite?.Invoke(this, PartyInvite);
            PartyInvite = null;
        }

        public void RevokePartyInvite(IPlayer invitedPlayer)
        {
            if (Party is null) return;
            Party.RevokeInvite(this, invitedPlayer);
            OnRevokePartyInvite?.Invoke(this, invitedPlayer, Party);
        }

        public void PartyEmptyHandler()
        {
            Party.OnPartyOver -= PartyEmptyHandler;
            LeaveParty();
        }

        public void LeaveParty()
        {
            if (Party is null) return;
            if (InFight) return;

            Party.OnPartyOver -= PartyEmptyHandler;

            if (IsPartyLeader) 
                Party.PassLeadership(this);

            Party?.RemoveMember(this);
            OnLeftParty?.Invoke(this, Party);
            Party = null;
        }

        public void JoinParty(IParty party)
        {
            if (party is null) return;
            if (Party is not null)
            {
                OperationFailService.Display(CreatureId, TextConstants.AlreadyInParty);
                return;
            }

            if (!party.JoinPlayer(this)) return;

            party.OnPartyOver += PartyEmptyHandler;

            Party = party;

            OnJoinedParty?.Invoke(this, party);
        }

        public void PassPartyLeadership(IPlayer player)
        {
            if (Party is null) return;

            var result = player.Party.ChangeLeadership(this, player);
            if (result.IsSuccess)
            {
                OnPassedPartyLeadership?.Invoke(this, player, Party);
                return;
            }

            switch (result.Error)
            {
                case InvalidOperation.NotAPartyMember:
                    OperationFailService.Display(CreatureId, TextConstants.PlayerIsNotPartyMember);
                    break;
                case InvalidOperation.NotAPartyLeader:
                    OperationFailService.Display(CreatureId, TextConstants.OnlyLeadersCanPassLeadership);
                    break;
            }
        }
    }
}