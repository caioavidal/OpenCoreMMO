using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Combat
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