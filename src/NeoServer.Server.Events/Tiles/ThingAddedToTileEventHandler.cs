using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Events
{
    public class ThingAddedToTileEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public ThingAddedToTileEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(IThing thing, ITile tile, byte toStackPosition)
        {
            foreach (var spectatorId in map.GetPlayersAtPositionZone(tile.Location))
            {
                IConnection connection = null;
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out connection))
                {
                    continue;
                }

                //if (thing is ICumulativeItem cumulative && cumulative.Amount > 0)
                //{
                //    connection.OutgoingPackets.Enqueue(new AddTileItemPacket(cumulative, fromStackPosition));
                //}
                //else
                //{
                //    connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(tile, fromStackPosition));
                //}

                connection.OutgoingPackets.Enqueue(new AddTileItemPacket((IItem)thing, toStackPosition));

                connection.Send();
            }
        }
    }
}
