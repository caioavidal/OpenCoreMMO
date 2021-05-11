using NeoServer.Game.Common.Location;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;
using System;

namespace NeoServer.Server.Commands.Player
{
    public class PlayerUseItemOnCommand : ICommand
    {
        private readonly IGameServer game;

        public PlayerUseItemOnCommand(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, UseItemOnPacket useItemPacket)
        {
            IItem onItem = null;
            ITile onTile = null;
            if (useItemPacket.ToLocation.Type == LocationType.Ground)
            {
                if (game.Map[useItemPacket.ToLocation] is not ITile tile) return;
                onTile = tile;
            }
            else if (useItemPacket.ToLocation.Type == LocationType.Slot)
            {
                if (player.Inventory[useItemPacket.ToLocation.Slot] is null) return;
                onItem = player.Inventory[useItemPacket.ToLocation.Slot];
            }
            else if (useItemPacket.ToLocation.Type == LocationType.Container)
            {
                if (player.Containers[useItemPacket.ToLocation.ContainerId][useItemPacket.ToLocation.ContainerSlot] is not IItem item) return;
                onItem = item;
            }

            if (onItem is not IItem && onTile is not ITile) return;

            Action action = null;

            IUseableOn itemToUse = null;

            if (useItemPacket.Location.Type == LocationType.Ground)
            {
                if (game.Map[useItemPacket.Location] is not ITile tile) return;

                if (tile.TopItemOnStack is IUseableOn useable)
                {
                    itemToUse = useable;
                }
            }
            else if (useItemPacket.Location.Type == LocationType.Slot)
            {
                if (player.Inventory[useItemPacket.Location.Slot] is not IUseableOn useable) return;

                itemToUse = useable;
            }
            else if (useItemPacket.Location.Type == LocationType.Container)
            {
                if (player.Containers[useItemPacket.Location.ContainerId][useItemPacket.Location.ContainerSlot] is not IUseableOn useable) return;
                itemToUse = useable;
            }

            action = onTile is not null ? () => player.Use(itemToUse, onTile) : () => player.Use(itemToUse, onItem);

            if (useItemPacket.Location.Type == LocationType.Ground)
            {
                action?.Invoke();
                return;
            }

            action?.Invoke();
        }

    }
}
