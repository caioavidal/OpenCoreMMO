using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Events
{
    public class ThingUpdatedOnTileEventHandler
    {
        private readonly Game game;

        public ThingUpdatedOnTileEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IThing thing, ICylinder cylinder)
        {
            cylinder.ThrowIfNull();
            cylinder.TileSpectators.ThrowIfNull();
            thing.ThrowIfNull();

            var tile = cylinder.ToTile;
            tile.ThrowIfNull();

            foreach (var spectator in cylinder.TileSpectators)
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.Spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new UpdateTileItemPacket(thing.Location, spectator.ToStackPosition, (IItem)thing));

                connection.Send();
            }
        }
    }
}