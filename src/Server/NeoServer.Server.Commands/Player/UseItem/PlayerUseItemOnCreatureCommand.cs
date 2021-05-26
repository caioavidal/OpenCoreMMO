using NeoServer.Game.Common.Location;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player.UseItem;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;
using System;

namespace NeoServer.Server.Commands.Player
{
    public class PlayerUseItemOnCreatureCommand : ICommand
    {
        private readonly IGameServer game;
        private readonly HotKeyCache hotKeyCache;

        public PlayerUseItemOnCreatureCommand(IGameServer game, HotKeyCache hotKeyCache)
        {
            this.game = game;
            this.hotKeyCache = hotKeyCache;
        }

        public void Execute(IPlayer player, UseItemOnCreaturePacket useItemPacket)
        {
            if (!game.CreatureManager.TryGetCreature(useItemPacket.CreatureId, out var creature)) return;

            IThing itemToUse = GetItem(player, useItemPacket);

            if (itemToUse is not IUseableOn useableOn) return;

            Action action = () => player.Use(useableOn, creature);

            if (useItemPacket.FromLocation.Type == LocationType.Ground)
            {
                action?.Invoke();
                return;
            }

            action?.Invoke();
        }

        private IThing GetItem(IPlayer player, UseItemOnCreaturePacket useItemPacket)
        {
            if (useItemPacket.FromLocation.IsHotkey)
            {
                var hotKeyItemLocation = hotKeyCache.Get(player.Id, useItemPacket.ClientId);
                if (hotKeyItemLocation is not null && hotKeyItemLocation.Container.RootParent is IPlayer owner)
                {

                    var containerItemId = hotKeyItemLocation.Container.Items.Count > hotKeyItemLocation.SlotIndex ? hotKeyItemLocation.Container[hotKeyItemLocation.SlotIndex]?.ClientId : null;

                    if (owner.Id == player.Id && containerItemId == useItemPacket.ClientId)
                    {
                        return hotKeyItemLocation.Container[hotKeyItemLocation.SlotIndex];
                    }
                }

                var foundItem = player.Inventory?.BackpackSlot?.GetFirstItem(useItemPacket.ClientId);// is not IThing thing) return null;
                if (foundItem?.Item1 is not IThing thing) return null;
                hotKeyCache.Add(player.Id, useItemPacket.ClientId, foundItem.Value.Item2, foundItem.Value.Item3);
                return thing;
            }

            if (useItemPacket.FromLocation.Type == LocationType.Ground)
            {
                if (game.Map[useItemPacket.FromLocation] is not ITile tile) return null;
                if (tile.TopItemOnStack is null) return null;
                return tile.TopItemOnStack;
            }

            if (useItemPacket.FromLocation.Type == LocationType.Slot)
            {
                if (player.Inventory[useItemPacket.FromLocation.Slot] is null) return null;
                return player.Inventory[useItemPacket.FromLocation.Slot];
            }

            if (useItemPacket.FromLocation.Type == LocationType.Container)
            {
                if (player.Containers[useItemPacket.FromLocation.ContainerId][useItemPacket.FromLocation.ContainerSlot] is not IThing thing) return null;
                return thing;
            }

            return null;
        }
    }
}