using System;
using System.Collections.Generic;
using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Schedulers;
using NeoServer.Server.Schedulers.Contracts;

namespace NeoServer.Game.Commands
{
    public class MapToMapMovementCommand : ICommand
    {
        public MapToMapMovementCommand(IThing thing, Location fromLocation, Location toLocation)
        {
            Thing = thing;
            FromLocation = fromLocation;
            ToLocation = toLocation;
        }

        public string EventId => Guid.NewGuid().ToString();

        public uint RequestorId { get; private set; }

        public string ErrorMessage => throw new System.NotImplementedException();

        public IThing Thing { get; }
        public Location FromLocation { get; }
        public Location ToLocation { get; }
    }
}
