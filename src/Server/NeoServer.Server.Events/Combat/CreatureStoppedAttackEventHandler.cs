using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Events
{
    public class CreatureStoppedAttackEventHandler
    {
        private readonly IGameServer game;

        public CreatureStoppedAttackEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(ICombatActor actor)
        {
            if (game.CreatureManager.GetPlayerConnection(actor.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new CancelTargetPacket());
                connection.Send();
            }
        }
    }
}