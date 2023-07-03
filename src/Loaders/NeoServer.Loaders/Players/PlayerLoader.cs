using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NeoServer.Data.Entities;
using NeoServer.Data.Parsers;
using NeoServer.Game.Chats;
using NeoServer.Game.Combat.Conditions;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Player;
using NeoServer.Game.Creatures.Player.Inventory;
using NeoServer.Loaders.Interfaces;
using Serilog;

namespace NeoServer.Loaders.Players;

public class PlayerLoader : IPlayerLoader
{
    protected readonly ChatChannelFactory _chatChannelFactory;
    protected readonly ICreatureFactory _creatureFactory;
    protected readonly IGuildStore _guildStore;
    protected readonly IItemFactory _itemFactory;
    protected readonly ILogger _logger;
    protected readonly IMapTool _mapTool;
    protected readonly IVocationStore _vocationStore;
    protected readonly Game.World.World _world;

    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public PlayerLoader(IItemFactory itemFactory, ICreatureFactory creatureFactory,
        ChatChannelFactory chatChannelFactory,
        IGuildStore guildStore,
        IVocationStore vocationStore,
        IMapTool mapTool,
        Game.World.World world,
        ILogger logger)
    {
        _itemFactory = itemFactory;
        _creatureFactory = creatureFactory;
        _chatChannelFactory = chatChannelFactory;
        _guildStore = guildStore;
        _vocationStore = vocationStore;
        _mapTool = mapTool;
        _world = world;
        _logger = logger;
    }

    public virtual bool IsApplicable(PlayerEntity player)
    {
        return player?.PlayerType == 1;
    }

    public virtual IPlayer Load(PlayerEntity playerEntity)
    {
        if (Guard.IsNull(playerEntity)) return null;

        var vocation = GetVocation(playerEntity);
        var town = GetTown(playerEntity);

        var playerLocation =
            new Location((ushort)playerEntity.PosX, (ushort)playerEntity.PosY, (byte)playerEntity.PosZ);

        var player = new Player(
            (uint)playerEntity.PlayerId,
            playerEntity.Name,
            playerEntity.ChaseMode,
            playerEntity.Capacity,
            playerEntity.Health,
            playerEntity.MaxHealth,
            vocation,
            playerEntity.Gender,
            playerEntity.Online,
            playerEntity.Mana,
            playerEntity.MaxMana,
            playerEntity.FightMode,
            playerEntity.Soul,
            vocation.SoulMax,
            ConvertToSkills(playerEntity),
            playerEntity.StaminaMinutes,
            new Outfit
            {
                Addon = (byte)playerEntity.LookAddons,
                Body = (byte)playerEntity.LookBody,
                Feet = (byte)playerEntity.LookFeet,
                Head = (byte)playerEntity.LookHead,
                Legs = (byte)playerEntity.LookLegs,
                LookType = (byte)playerEntity.LookType
            },
            0,
            playerLocation,
            _mapTool,
            town)
        {
            PremiumTime = playerEntity.Account?.PremiumTime ?? 0,
            AccountId = (uint)playerEntity.AccountId,
            Guild = _guildStore.Get((ushort)(playerEntity.GuildMember?.GuildId ?? 0)),
            GuildLevel = (ushort)(playerEntity.GuildMember?.RankId ?? 0)
        };

        SetCurrentTile(player);
        AddRegenerationCondition(playerEntity, player);

        player.AddInventory(ConvertToInventory(player, playerEntity));

        AddExistingPersonalChannels(player);

        return _creatureFactory.CreatePlayer(player);
    }

    protected ITown GetTown(PlayerEntity playerEntity)
    {
        if (!_world.TryGetTown((ushort)playerEntity.TownId, out var town))
            _logger.Error("player town not found: {PlayerModelTownId}", playerEntity.TownId);
        return town;
    }

    protected IVocation GetVocation(PlayerEntity playerEntity)
    {
        if (!_vocationStore.TryGetValue(playerEntity.Vocation, out var vocation))
            _logger.Error("Player vocation not found: {PlayerModelVocation}", playerEntity.Vocation);
        return vocation;
    }

    protected void SetCurrentTile(IPlayer player)
    {
        var location = player.Location;
        player.SetCurrentTile(_world.TryGetTile(ref location, out var tile) && tile is IDynamicTile dynamicTile
            ? dynamicTile
            : null);
    }

    private static void AddRegenerationCondition(PlayerEntity playerEntity, IPlayer player)
    {
        if (playerEntity.RemainingRecoverySeconds != 0)
        {
            player.AddCondition(
                new Condition(ConditionType.Regeneration, (uint)(playerEntity.RemainingRecoverySeconds * 1000),
                    player.SetAsHungry));
            return;
        }

        player.SetAsHungry();
    }

    /// <summary>
    ///     Adds all PersonalChatChannel assemblies to Player
    /// </summary>
    protected virtual void AddExistingPersonalChannels(IPlayer player)
    {
        if (player is null) return;

        var personalChannels = GameAssemblyCache.Cache
            .Where(x => typeof(PersonalChatChannel).IsAssignableFrom(x));
        foreach (var channel in personalChannels)
        {
            if (channel == typeof(PersonalChatChannel)) continue;

            var createdChannel = _chatChannelFactory.Create(channel, null, player);
            player.Channels.AddPersonalChannel(createdChannel);
        }
    }

    protected static Dictionary<SkillType, ISkill> ConvertToSkills(PlayerEntity playerRecord)
    {
        return new Dictionary<SkillType, ISkill>
        {
            [SkillType.Axe] = new Skill(SkillType.Axe, (ushort)playerRecord.SkillAxe, playerRecord.SkillAxeTries),
            [SkillType.Club] = new Skill(SkillType.Club, (ushort)playerRecord.SkillClub, playerRecord.SkillClubTries),
            [SkillType.Distance] = new Skill(SkillType.Distance, (ushort)playerRecord.SkillDist,
                playerRecord.SkillDistTries),
            [SkillType.Fishing] = new Skill(SkillType.Fishing, (ushort)playerRecord.SkillFishing,
                playerRecord.SkillFishingTries),
            [SkillType.Fist] = new Skill(SkillType.Fist, (ushort)playerRecord.SkillFist, playerRecord.SkillFistTries),
            [SkillType.Shielding] = new Skill(SkillType.Shielding, (ushort)playerRecord.SkillShielding,
                playerRecord.SkillShieldingTries),
            [SkillType.Level] = new Skill(SkillType.Level, playerRecord.Level, playerRecord.Experience),
            [SkillType.Magic] =
                new Skill(SkillType.Magic, (ushort)playerRecord.MagicLevel, playerRecord.MagicLevelTries),
            [SkillType.Sword] =
                new Skill(SkillType.Sword, (ushort)playerRecord.SkillSword, playerRecord.SkillSwordTries)
        };
    }

    protected IInventory ConvertToInventory(IPlayer player, PlayerEntity playerRecord)
    {
        var inventory = new Dictionary<Slot, (IItem Item, ushort Id)>();
        var attrs = new Dictionary<ItemAttribute, IConvertible> { { ItemAttribute.Count, 0 } };

        foreach (var item in playerRecord.PlayerInventoryItems)
        {
            attrs[ItemAttribute.Count] = (byte)item.Amount;
            var location = item.SlotId <= 10 ? Location.Inventory((Slot)item.SlotId) : Location.Container(0, 0);

            var createdItem = _itemFactory.Create((ushort)item.ServerId, location, attrs);
            var createdItemIsPickupable = createdItem?.IsPickupable ?? false;

            if (!createdItemIsPickupable) continue;

            if (item.SlotId == (int)Slot.Backpack)
            {
                if (createdItem is not IContainer container) continue;

                ItemEntityParser.BuildContainer(container, playerRecord.PlayerItems.ToList(), location, _itemFactory);
            }

            switch (item.SlotId)
            {
                case (int)Slot.Necklace:
                    inventory.Add(Slot.Necklace, (createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Head:
                    inventory.Add(Slot.Head, (createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Backpack:
                    inventory.Add(Slot.Backpack, (createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Left:
                    inventory.Add(Slot.Left, (createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Body:
                    inventory.Add(Slot.Body, (createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Right:
                    inventory.Add(Slot.Right, (createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Ring:
                    inventory.Add(Slot.Ring, (createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Legs:
                    inventory.Add(Slot.Legs, (createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Ammo:
                    inventory.Add(Slot.Ammo, (createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Feet:
                    inventory.Add(Slot.Feet, (createdItem, (ushort)item.ServerId));
                    break;
            }
        }

        return new Inventory(player, inventory);
    }
}