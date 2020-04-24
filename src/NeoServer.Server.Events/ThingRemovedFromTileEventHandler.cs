using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class ThingRemovedFromTileEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public ThingRemovedFromTileEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(NeoServer.Game.Contracts.Items.IThing thing, ITile tile, byte fromStackPosition)
        {
            var player = thing as IPlayer;

            foreach (var spectatorId in map.GetCreaturesAtPositionZone(tile.Location))
            {
                IConnection connection = null;
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out connection))
                {
                    continue;
                }

                if (!player.IsDead)
                {
                    connection.OutgoingPackets.Enqueue(new MagicEffectPacket(tile.Location, EffectT.Puff));
                }

                connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(tile, fromStackPosition));

                connection.Send();
            }
        }
    }
}
