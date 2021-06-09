using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Events
{
    public class ItemUsedOnTileEventHandler
    {
        private readonly IGameServer game;

        public ItemUsedOnTileEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(ICreature usedBy, ITile onTile, IUseableOnTile item)
        {
            foreach (var spectator in game.Map.GetPlayersAtPositionZone(usedBy.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

                if (item.Metadata.ShootType != default)
                    connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(usedBy.Location, onTile.Location,
                        (byte) item.Metadata.ShootType));
                connection.Send();
            }
        }
    }
}