using System.Collections.Generic;
using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Events;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Schedulers.Contracts;

namespace NeoServer.Server.Events
{
    public class ThingRemovedFromTileEventHandler : IEventHandler<ThingRemovedFromTileEvent>
    {
        private readonly IMap map;
        private readonly Game game;

        public ThingRemovedFromTileEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(ThingRemovedFromTileEvent evt)
        {
            var outgoingPackets = new Queue<IOutgoingPacket>();
            foreach (var spectatorId in map.GetCreaturesAtPositionZone(evt.Tile.Location))
            {
                if (game.Connections.TryGetValue(spectatorId, out IConnection connection))
                {
                    outgoingPackets.Enqueue(new RemoveTileThingPacket(evt.Tile, evt.StackPosition));
                    outgoingPackets.Enqueue(new MagicEffectPacket(evt.Tile.Location, EffectT.Puff));

                    connection.Send(outgoingPackets, true);
                }
            }
        }
    }
}
