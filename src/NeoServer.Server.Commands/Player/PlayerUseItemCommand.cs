using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Players;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;

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
                if(tile.TopItemOnStack is IContainer container)
                {
                    player.Containers.OpenContainerAt(useItemPacket.Location, useItemPacket.Index, container);
                    return;
                }
                if(tile.TopItemOnStack is IUseable useable)
                {
                    useable.Use(player, game.Map);
                }
            }
            else if (useItemPacket.Location.Slot == Slot.Backpack || useItemPacket.Location.Type == LocationType.Container)
            {
                player.Containers.OpenContainerAt(useItemPacket.Location, useItemPacket.Index);
            }
        }

    }
}
