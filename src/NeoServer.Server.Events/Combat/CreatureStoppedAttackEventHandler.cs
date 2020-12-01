using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Events
{
    public class CreatureStoppedAttackEventHandler
    {
        private readonly Game game;

        public CreatureStoppedAttackEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICombatActor actor)
        {
            if (game.CreatureManager.GetPlayerConnection(actor.CreatureId, out IConnection connection))
            {
                connection.OutgoingPackets.Enqueue(new CancelTargetPacket());
                connection.Send();
            }
        }
    }
}
