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
        private readonly HotkeyService hotKeyService;

        public PlayerUseItemOnCreatureCommand(IGameServer game, HotkeyService hotKeyCache)
        {
            this.game = game;
            this.hotKeyService = hotKeyCache;
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
                return hotKeyService.GetItem(player, useItemPacket.ClientId);
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