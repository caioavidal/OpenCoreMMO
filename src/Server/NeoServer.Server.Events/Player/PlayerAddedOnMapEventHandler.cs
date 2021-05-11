using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Events
{
    public class PlayerAddedOnMapEventHandler : IEventHandler
    {
        private readonly IMap map;
        private readonly IGameServer game;

        public PlayerAddedOnMapEventHandler(IMap map, IGameServer game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(IWalkableCreature creature, ICylinder cylinder)
        {
            if (Guard.AnyNull(cylinder, cylinder.TileSpectators, creature)) return;

            var tile = cylinder.ToTile;
            if (tile.IsNull()) return;

            foreach (var cylinderSpectator in cylinder.TileSpectators)
            {
                var spectator = cylinderSpectator.Spectator;

                if (spectator is not IPlayer spectatorPlayer) continue;
                if (creature == spectator) continue;

                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

                SendPacketsToSpectator(spectatorPlayer, creature, connection, cylinderSpectator.ToStackPosition);

                connection.Send();
            }
        }

        private void SendPacketsToSpectator(IPlayer playerToSend, IWalkableCreature creatureAdded, IConnection connection, byte stackPosition)
        {
            connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(creatureAdded, stackPosition));
            connection.OutgoingPackets.Enqueue(new AddCreaturePacket(playerToSend, creatureAdded));
            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creatureAdded.Location, EffectT.BubbleBlue));
        }
    }
}
