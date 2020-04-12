using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerThrowItemCommand : Command
    {
        private readonly Game game;
        private ItemThrowPacket itemThrow;
        private readonly IPlayer player;

        public PlayerThrowItemCommand(IPlayer player, ItemThrowPacket itemThrow, Game game)
        {
            this.itemThrow = itemThrow;
            this.game = game;
            this.player = player;
        }

        public override void Execute()
        {
           
        }

      
    }
}
