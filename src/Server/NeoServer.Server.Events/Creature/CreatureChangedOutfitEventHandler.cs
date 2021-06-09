using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureChangedOutfitEventHandler
    {
        private readonly IGameServer game;
        private readonly IMap map;

        public CreatureChangedOutfitEventHandler(IMap map, IGameServer game)
        {
            this.map = map;
            this.game = game;
        }

        public void Execute(ICreature creature, IOutfit outfit)
        {
            foreach (var spectator in map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!creature.CanSee(spectator.Location)) continue;

                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;
                connection.OutgoingPackets.Enqueue(new CreatureOutfitPacket(creature));
                connection.Send();
            }
        }
    }
}