using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Networking.Packets.Outgoing.Item;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Player
{
    public class PlayerAddedOnMapEventHandler : IEventHandler
    {
        private readonly IGameServer game;
        private readonly IMap map;

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

        private void SendPacketsToSpectator(IPlayer playerToSend, IWalkableCreature creatureAdded,
            IConnection connection, byte stackPosition)
        {
            connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(creatureAdded, stackPosition));
            connection.OutgoingPackets.Enqueue(new AddCreaturePacket(playerToSend, creatureAdded));
            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creatureAdded.Location, EffectT.BubbleBlue));
        }
    }
}