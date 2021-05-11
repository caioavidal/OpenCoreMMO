using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;
using System;

namespace NeoServer.Server.Commands.Player
{
    public class PlayerUseItemCommand : ICommand
    {
        private readonly IGameServer game;

        public PlayerUseItemCommand(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, UseItemPacket useItemPacket)
        {
            IItem item = null;
            if (useItemPacket.Location.Type == LocationType.Ground)
            {
                if (game.Map[useItemPacket.Location] is not ITile tile) return;
                item = tile.TopItemOnStack;
            }
            else if (useItemPacket.Location.Slot == Slot.Backpack)
            {
                item = player.Inventory[Slot.Backpack];
                item.Location = useItemPacket.Location;
            }
            else if (useItemPacket.Location.Type == LocationType.Container)
            {
                item = player.Containers[useItemPacket.Location.ContainerId][useItemPacket.Location.ContainerSlot];
                item.Location = useItemPacket.Location;
            }

            Action action = null;

            if (item is null) return;

            if (item is IContainer container)
            {
                action = () => player.Containers.OpenContainerAt(useItemPacket.Location, useItemPacket.Index, container);
            }
            else if (item is IUseable useable)
            {
                action = () => player.Use(useable);
            }
            else if (item is IUseableOn useableOn)
            {
                action = () => player.Use(useableOn, player);
            }

            if (action is null) return;

            if (useItemPacket.Location.Type == LocationType.Ground)
            {

                action?.Invoke();
                return;
            }

            action?.Invoke();
        }
    }
}
