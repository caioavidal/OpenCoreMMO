using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
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
            foreach (var spectatorId in map.GetPlayersAtPositionZone(tile.Location))
            {
                IConnection connection = null;
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out connection))
                {
                    continue;
                }

                if (thing is IPlayer player && !player.IsDead)
                {
                    connection.OutgoingPackets.Enqueue(new MagicEffectPacket(tile.Location, EffectT.Puff));
                }

                if (thing is ICumulativeItem cumulative && cumulative.Amount > 0)
                {
                    connection.OutgoingPackets.Enqueue(new UpdateTileItemPacket(tile.Location, fromStackPosition, cumulative));
                }
                else
                {
                    connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(tile, fromStackPosition));
                }

                connection.Send();
            }
        }
    }
}
