using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerChangedOnlineStatusEventHandler : IEventHandler
    {
        private readonly IGameServer game;
        private readonly IMap map;

        public PlayerChangedOnlineStatusEventHandler(IMap map, IGameServer game)
        {
            this.map = map;
            this.game = game;
        }

        public void Execute(IPlayer player, bool online)
        {
            if (player.IsNull()) return;

            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

            foreach (var loggedPlayer in game.CreatureManager.GetAllLoggedPlayers())
            {
                if (!loggedPlayer.HasInVipList(player.Id)) continue;
                if (!game.CreatureManager.GetPlayerConnection(loggedPlayer.CreatureId, out var friendConnection))
                    continue;

                friendConnection.OutgoingPackets.Enqueue(new PlayerUpdateVipStatusPacket(player.Id, online));
                friendConnection.Send();
            }
        }
    }
}