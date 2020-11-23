using NeoServer.Game.Contracts.Items;
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

            var tile = cylinder.ToTile;
            tile.ThrowIfNull();

            var spectators = cylinder.TileSpectators;

            foreach (var spectator in spectators)
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.Spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }
                if (!(spectator.Spectator is IPlayer))
                {
                    continue;
                }


                connection.OutgoingPackets.Enqueue(new RemoveTileItemPacket(cylinder.FromTile.Location, spectator.ToStackPosition, item));
                connection.OutgoingPackets.Enqueue(new AddTileItemPacket(item, spectator.ToStackPosition));

                connection.Send();
            }
        }
    }
}
