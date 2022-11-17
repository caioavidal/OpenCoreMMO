using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NeoServer.Data.Model;
using NeoServer.Game.Chats;
using NeoServer.Game.Combat.Conditions;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Player;
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
    protected readonly IWalkToMechanism _walkToMechanism;
    protected readonly Game.World.World _world;

    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public PlayerLoader(IItemFactory itemFactory, ICreatureFactory creatureFactory,
        ChatChannelFactory chatChannelFactory,
        IGuildStore guildStore,
        IVocationStore vocationStore,
        IMapTool mapTool,
        IWalkToMechanism walkToMechanism,
        Game.World.World world,
        ILogger logger)
    {
        _itemFactory = itemFactory;
        _creatureFactory = creatureFactory;
        _chatChannelFactory = chatChannelFactory;
        _guildStore = guildStore;
        _vocationStore = vocationStore;
        _mapTool = mapTool;
        _walkToMechanism = walkToMechanism;
        _world = world;
        _logger = logger;
    }

    public virtual bool IsApplicable(PlayerModel player)
    {
        return player.PlayerType == 1;
    }

    public virtual IPlayer Load(PlayerModel playerModel)
    {
        if (!_vocationStore.TryGetValue(playerModel.Vocation, out var vocation))
            _logger.Error($"Player vocation not found: {playerModel.Vocation}");

        if (!_world.TryGetTown((ushort)playerModel.TownId, out var town))
            _logger.Error($"Town of player not found: {playerModel.TownId}");

        var playerLocation = new Location((ushort)playerModel.PosX, (ushort)playerModel.PosY, (byte)playerModel.PosZ);

        var player = new Player(
            (uint)playerModel.PlayerId,
            playerModel.Name,
            playerModel.ChaseMode,
            playerModel.Capacity,
            playerModel.Health,
            playerModel.MaxHealth,
            vocation,
            playerModel.Gender,
            playerModel.Online,
            playerModel.Mana,
            playerModel.MaxMana,
            playerModel.FightMode,
            playerModel.Soul,
            vocation.SoulMax,
            ConvertToSkills(playerModel),
            playerModel.StaminaMinutes,
            new Outfit
            {
                Addon = (byte)playerModel.LookAddons,
                Body = (byte)playerModel.LookBody,
                Feet = (byte)playerModel.LookFeet,
                Head = (byte)playerModel.LookHead,
                Legs = (byte)playerModel.LookLegs,
                LookType = (byte)playerModel.LookType
            },
            0,
            playerLocation,
            _mapTool,
            town,
            playerModel.Account.PremiumTime
        )
        {
            AccountId = (uint)playerModel.AccountId,
            Guild = _guildStore.Get((ushort)(playerModel.GuildMember?.GuildId ?? 0)),
            GuildLevel = (ushort)(playerModel.GuildMember?.RankId ?? 0),
        };

        SetCurrentTile(player);
        AddRegenerationCondition(playerModel, player);

        player.AddInventory(ConvertToInventory(player, playerModel));

        AddExistingPersonalChannels(player);

        return _creatureFactory.CreatePlayer(player);
    }
    
    protected void SetCurrentTile(IPlayer player)
    {
        var location = player.Location;
        player.SetCurrentTile(_world.TryGetTile(ref location, out var tile) && tile is IDynamicTile dynamicTile ? dynamicTile : null);
    }

    private static void AddRegenerationCondition(PlayerModel playerModel, IPlayer player)
    {
        if (playerModel.RemainingRecoverySeconds != 0)
        {
            player.AddCondition(
                new Condition(ConditionType.Regeneration, (uint)(playerModel.RemainingRecoverySeconds * 1000),
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

        var personalChannels = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => typeof(PersonalChatChannel).IsAssignableFrom(x));
        foreach (var channel in personalChannels)
        {
            if (channel == typeof(PersonalChatChannel)) continue;

            var createdChannel = _chatChannelFactory.Create(channel, null, player);
            player.Channels.AddPersonalChannel(createdChannel);
        }
    }

    protected static Dictionary<SkillType, ISkill> ConvertToSkills(PlayerModel playerRecord)
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

    protected IInventory ConvertToInventory(IPlayer player, PlayerModel playerRecord)
    {
        var inventory = new Dictionary<Slot, Tuple<IPickupable, ushort>>();
        var attrs = new Dictionary<ItemAttribute, IConvertible> { { ItemAttribute.Count, 0 } };

        foreach (var item in playerRecord.PlayerInventoryItems)
        {
            attrs[ItemAttribute.Count] = (byte)item.Amount;
            var location = item.SlotId <= 10 ? Location.Inventory((Slot)item.SlotId) : Location.Container(0, 0);

            if (_itemFactory.Create((ushort)item.ServerId, location, attrs) is not IPickupable createdItem) continue;

            if (item.SlotId == (int)Slot.Backpack)
            {
                if (createdItem is not IContainer container) continue;
                BuildContainer(playerRecord.PlayerItems.Where(c => c.ParentId.Equals(0)).ToList(), 0, location,
                    container, playerRecord.PlayerItems.ToList());
            }

            switch (item.SlotId)
            {
                case (int)Slot.Necklace:
                    inventory.Add(Slot.Necklace, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Head:
                    inventory.Add(Slot.Head, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Backpack:
                    inventory.Add(Slot.Backpack, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Left:
                    inventory.Add(Slot.Left, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Body:
                    inventory.Add(Slot.Body, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Right:
                    inventory.Add(Slot.Right, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Ring:
                    inventory.Add(Slot.Ring, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Legs:
                    inventory.Add(Slot.Legs, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Ammo:
                    inventory.Add(Slot.Ammo, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                    break;
                case (int)Slot.Feet:
                    inventory.Add(Slot.Feet, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                    break;
            }
        }

        return new Inventory(player, inventory);
    }

    private IContainer BuildContainer(IReadOnlyList<PlayerItemModel> items, int index, Location location,
        IContainer container, IEnumerable<PlayerItemModel> all)
    {
        while (true)
        {
            if (items == null || items.Count == index) return container;

            all = all.ToList();

            var itemModel = items[index];

            var item = _itemFactory.Create((ushort)itemModel.ServerId, location,
                new Dictionary<ItemAttribute, IConvertible> { { ItemAttribute.Count, (byte)itemModel.Amount } });

            if (item is IContainer childrenContainer)
            {
                childrenContainer.SetParent(container);
                container.AddItem(BuildContainer(all.Where(c => c.ParentId.Equals(itemModel.Id)).ToList(), 0, location,
                    childrenContainer, all));
            }
            else
            {
                container.AddItem(item);
            }

            index = ++index;
        }
    }
}