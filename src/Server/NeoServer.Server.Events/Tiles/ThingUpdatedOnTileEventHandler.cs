using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Events
{
    public class ThingUpdatedOnTileEventHandler
    {
        private readonly IGameServer game;

        public ThingUpdatedOnTileEventHandler(IGameServer game)
        {
            this.game = game;
        }
        public void Execute(IThing thing, ICylinder cylinder)
        {
            if (Guard.AnyNull(cylinder, cylinder.TileSpectators, thing)) return;

            var tile = cylinder.ToTile;
            if (tile.IsNull()) return;

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