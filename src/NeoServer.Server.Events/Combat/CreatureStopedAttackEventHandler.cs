using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;


namespace NeoServer.Server.Events
{
    public class CreatureStoppedAttackEventHandler
    {
        private readonly Game game;

        public CreatureStoppedAttackEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature actor)
        {
            if (game.CreatureManager.GetPlayerConnection(actor.CreatureId, out IConnection connection))
            {
                connection.OutgoingPackets.Enqueue(new CancelTargetPacket());
                connection.Send();
            }
        }
    }
}
