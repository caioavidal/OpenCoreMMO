using System.Collections.Generic;
using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Commands
{
    public class MapToMapMovementCommand : Command
    {
        private IThing thing;
        private readonly Location fromLocation;
        private readonly Location toLocation;
        private readonly Game game;
        public MapToMapMovementCommand(IThing thing, Location fromLocation, Location toLocation, Game game)
        {
            this.thing = thing;
            this.fromLocation = fromLocation;
            this.toLocation = toLocation;
            this.game = game;
        }


        public override void Execute()
        {
            game.Map.MoveThing(ref thing, toLocation, 1);
        }

      
    }
}
