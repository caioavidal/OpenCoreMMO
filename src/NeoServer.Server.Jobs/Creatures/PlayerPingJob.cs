using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;

namespace NeoServer.Server.Jobs
{
    public class PlayerPingJob
    {
        private const int PING_INTERVAL = 5000;
        private const int CONNECTION_LOST_INTERVAL = 60000;
        public static void Execute(IPlayer player, Game game)
        {
            if (player.IsDead)
            {
                return;
            }

            var now = DateTime.Now.Ticks;

            var hasLostConnnection = false;

            IConnection connection = null;

            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out connection))
            {
                return;
            }


            var remainingTime = TimeSpan.FromTicks(now - connection.LastPingRequest).TotalMilliseconds;

            if (remainingTime >= PING_INTERVAL)
            {
                connection.LastPingRequest = now;

                connection.Send(new PingPacket());
            }

            //todo
            //int64_t noPongTime = timeNow - lastPong;
            //if ((hasLostConnection || noPongTime >= 7000) && attackedCreature && attackedCreature->getPlayer())
            //{
            //    setAttackedCreature(nullptr);
            //}

            var noPongTime = TimeSpan.FromTicks(now - connection.LastPingResponse).TotalMilliseconds;

            if (noPongTime >= CONNECTION_LOST_INTERVAL && player.CanLogout && connection.LastPingResponse > 0)
            {
                new PlayerLogOutCommand(player, game, true).Execute();
            }
        }
    }
}
