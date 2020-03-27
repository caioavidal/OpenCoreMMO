using System;
using System.Collections.Generic;
using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Schedulers;

namespace NeoServer.Game.Commands
{
    public class MapToMapMovementCommand : IEvent
    {
        public string EventId => Guid.NewGuid().ToString();

        public uint RequestorId { get; private set; }

        public string ErrorMessage => throw new System.NotImplementedException();

        private readonly IThing thing;
        private readonly Location fromLocation;
        private readonly Location toLocation;

        private readonly IMap map;
        private readonly IConnection connection;

        public MapToMapMovementCommand(IThing thing, Location fromLocation, Location toLocation, IMap map, IConnection connection)
        {
            this.thing = thing;
            this.fromLocation = fromLocation;
            this.toLocation = toLocation;
            this.map = map;
            this.connection = connection;
        }

        public void Execute()
        {
            var fromStackPosition = map[fromLocation].GetStackPositionOfThing(thing);

            var outgoingPackets = new Queue<IOutgoingPacket>();

            var direction = fromLocation.DirectionTo(toLocation);

            outgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation, fromStackPosition));
            outgoingPackets.Enqueue(new MapPartialDescriptionPacket(thing, toLocation, direction, map));

            connection.Send(outgoingPackets);
        }
    }
}
