using NeoServer.Data.Interfaces;
using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerChangedOnlineStatusEventHandler : IEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public PlayerChangedOnlineStatusEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(IPlayer player, bool online)
        {
            player.ThrowIfNull();

            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

            foreach (var loggedPlayer in game.CreatureManager.GetAllLoggedPlayers())
            {
                if (!loggedPlayer.HasInVipList(player.Id)) continue;
                if (!game.CreatureManager.GetPlayerConnection(loggedPlayer.CreatureId, out var friendConnection)) continue;

                friendConnection.OutgoingPackets.Enqueue(new PlayerUpdateVipStatusPacket(player.Id, online));
                friendConnection.Send();
            }
        }

    }
}
