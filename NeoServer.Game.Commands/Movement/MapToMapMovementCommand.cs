using System;
using System.Collections.Generic;
using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
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

        private IThing thing;
        private readonly ITile fromTile;
        private readonly ITile toTile;
        private readonly Location fromLocation;
        private readonly Location toLocation;

        private readonly IMap map;
        private readonly IConnection connection;

        public MapToMapMovementCommand(IThing thing, Location fromLocation, Location toLocation, IMap map, IConnection connection)
        {
            this.thing = thing;
            this.fromTile = map[fromLocation];
            this.toTile = map[toLocation];
            this.map = map;
            this.connection = connection;

            this.fromLocation = fromLocation;
            this.toLocation = toLocation;
        }

        public MapToMapMovementCommand(IThing thing, Location fromLocation, Direction direction, IMap map, IConnection connection)
        {
            this.thing = thing;
            this.fromTile = map[fromLocation];
            this.toTile = map.GetNextTile(fromLocation, direction);
            this.map = map;
            this.connection = connection;

            this.fromLocation = fromLocation;
            this.toLocation = toTile.Location;
        }

        public void Execute()
        {
            var fromStackPosition = fromTile.GetStackPositionOfThing(thing);

            fromTile.RemoveThing(ref thing, 1);
            toTile.AddThing(ref thing, 1);

            var outgoingPackets = new Queue<IOutgoingPacket>();

            var direction = fromLocation.DirectionTo(toLocation);

            Console.WriteLine($"Added thing at {toLocation}");

            outgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation, fromStackPosition));
            outgoingPackets.Enqueue(new MapPartialDescriptionPacket(thing, toLocation, direction, map));

            connection.Send(outgoingPackets);
        }
    }
}
