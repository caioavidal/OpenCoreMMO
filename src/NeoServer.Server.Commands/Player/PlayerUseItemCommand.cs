using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Players;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
using NeoServer.Server.Commands.Movement;
using System;

namespace NeoServer.Server.Commands.Player
{
    public class PlayerUseItemCommand : Command
    {
        private readonly Game game;
        private UseItemPacket useItemPacket;
        private readonly IPlayer player;

        public PlayerUseItemCommand(IPlayer player, Game game, UseItemPacket useItemPacket)
        {
            this.game = game;
            this.player = player;
            this.useItemPacket = useItemPacket;
        }

        public override void Execute()
        {
            if (useItemPacket.Location.Type == LocationType.Ground)
            {
                if (game.Map[useItemPacket.Location] is not ITile tile) return;

                Action action = null;

                if(tile.TopItemOnStack is IContainer container)
                {
                    action = () => player.Containers.OpenContainerAt(useItemPacket.Location, useItemPacket.Index, container);
                }
                else if(tile.TopItemOnStack is IUseable useable)
                {
                    action = () => useable.Use(player, game.Map);
                }

                WalkToMechanism.DoOperation(player, action, useItemPacket.Location, game);
            }
            else if (useItemPacket.Location.Slot == Slot.Backpack || useItemPacket.Location.Type == LocationType.Container)
            {
                player.Containers.OpenContainerAt(useItemPacket.Location, useItemPacket.Index);
            }
        }
    }
}
