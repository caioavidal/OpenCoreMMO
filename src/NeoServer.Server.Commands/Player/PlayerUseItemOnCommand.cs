using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Contracts.Items;
using System;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerUseItemOnCommand : Command
    {
        private readonly Game game;
        private UseItemOnPacket useItemPacket;
        private readonly IPlayer player;

        public PlayerUseItemOnCommand(IPlayer player, Game game, UseItemOnPacket useItemPacket)
        {
            this.game = game;
            this.player = player;
            this.useItemPacket = useItemPacket;
        }

        public override void Execute()
        {
            IThing onThing = null;
            if (useItemPacket.ToLocation.Type == LocationType.Ground)
            {
                if (game.Map[useItemPacket.ToLocation] is not ITile tile) return;
                if (tile.TopItemOnStack is null) return;
                onThing = tile.TopItemOnStack;
            }
            else if (useItemPacket.ToLocation.Type == LocationType.Slot)
            {
                if (player.Inventory[useItemPacket.ToLocation.Slot] is null) return;
                onThing = player.Inventory[useItemPacket.ToLocation.Slot];
            }
            else if (useItemPacket.ToLocation.Type == LocationType.Container)
            {
                if (player.Containers[useItemPacket.ToLocation.ContainerId][useItemPacket.ToLocation.ContainerSlot] is not IThing thing) return;
                onThing = thing;
            }

            if (onThing is not IThing) return;
            Action action = null;

            
            if (useItemPacket.Location.Type == LocationType.Ground)
            {
            //    action
                if (game.Map[useItemPacket.Location] is not ITile tile) return;
          
                if(tile.TopItemOnStack is IUseableOn useable)
                {
                    useable.UseOn(player, game.Map, onThing);
                }
            }
            else if (useItemPacket.Location.Type ==  LocationType.Slot)
            {
                if (player.Inventory[useItemPacket.Location.Slot] is not IUseableOn useable) return;
                useable.UseOn(player, game.Map, onThing);
            }
            else if (useItemPacket.Location.Type == LocationType.Container)
            {
                if (player.Containers[useItemPacket.Location.ContainerId][useItemPacket.Location.ContainerSlot] is not IUseableOn useable) return;
                useable.UseOn(player, game.Map, onThing);
            }
        }

    }
}
