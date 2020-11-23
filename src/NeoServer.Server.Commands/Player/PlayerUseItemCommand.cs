using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Players;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;

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
                
                if (!(game.Map[useItemPacket.Location] is IDynamicTile tile)) return;

                var toTile = game.Map.GetTileDestination(tile);
                if (toTile is null) return;

                var thing = player as IMoveableThing;
                game.Map.TryMoveThing(thing, toTile.Location);

            }
            if (useItemPacket.Location.Type == LocationType.Ground || useItemPacket.Location.Slot == Slot.Backpack || useItemPacket.Location.Type == LocationType.Container)
            {
                player.Containers.OpenContainerAt(useItemPacket.Location, useItemPacket.Index);
            }
        }

    }
}
