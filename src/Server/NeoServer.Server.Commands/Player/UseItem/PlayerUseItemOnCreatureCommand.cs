using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Commands;

namespace NeoServer.Server.Commands.Player.UseItem
{
    public class PlayerUseItemOnCreatureCommand : ICommand
    {
        private readonly IGameServer game;
        private readonly HotkeyService hotKeyService;

        public PlayerUseItemOnCreatureCommand(IGameServer game, HotkeyService hotKeyCache)
        {
            this.game = game;
            hotKeyService = hotKeyCache;
        }

        public void Execute(IPlayer player, UseItemOnCreaturePacket useItemPacket)
        {
            if (!game.CreatureManager.TryGetCreature(useItemPacket.CreatureId, out var creature)) return;

            var itemToUse = GetItem(player, useItemPacket);

            if (itemToUse is not IUsableOn useableOn) return;

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
            if (useItemPacket.FromLocation.IsHotkey) return hotKeyService.GetItem(player, useItemPacket.ClientId);

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
                if (player.Containers[useItemPacket.FromLocation.ContainerId][useItemPacket.FromLocation.ContainerSlot]
                    is not IThing thing) return null;
                return thing;
            }

            return null;
        }
    }
}