using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Events.Tiles
{
    class ItemMovedToTileEventHandler
    {
        private readonly Game game;

        public ItemMovedToTileEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IItem item, ICylinder cylinder)
        {
            cylinder.ThrowIfNull();
            cylinder.TileSpectators.ThrowIfNull();
            item.ThrowIfNull();


            var spectators = cylinder.TileSpectators;

            foreach (var spectator in spectators)
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.Spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }
             
                if (item is ICumulativeItem cumulative && cumulative.Amount > 0)
                {
                    connection.OutgoingPackets.Enqueue(new UpdateTileItemPacket(cylinder.FromTile.Location, spectator.FromStackPosition, cumulative));
                }
                else
                {
                    connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(cylinder.FromTile, spectator.FromStackPosition));
                }

                connection.OutgoingPackets.Enqueue(new AddTileItemPacket(item, spectator.ToStackPosition));

                connection.Send();
            }
        }
    }
}
