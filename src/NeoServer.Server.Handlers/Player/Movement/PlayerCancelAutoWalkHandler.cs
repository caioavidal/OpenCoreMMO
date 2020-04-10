﻿using NeoServer.Game.Contracts;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerCancelAutoWalkHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IMap map;



        public PlayerCancelAutoWalkHandler(Game game, IMap map)
        {
            this.game = game;
            this.map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var autoWalk = new AutoWalkPacket(message);

            var player = game.CreatureManager.GetCreature(connection.PlayerId) as IPlayer;

            game.Dispatcher.AddEvent(new Event(player.StopWalking));
        }
    }
}
