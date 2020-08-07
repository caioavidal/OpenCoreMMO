using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class ThingUpdatedOnTileEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public ThingUpdatedOnTileEventHandler(IMap map, Game game)
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

                connection.OutgoingPackets.Enqueue(new UpdateTileItemPacket(thing.Location, toStackPosition, (IItem)thing));

                connection.Send();
            }
        }
    }
}
