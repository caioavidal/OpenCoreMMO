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
    public class MapToMapMovementCommandHandler : ICommandHandler<MapToMapMovementCommand>
    {
        private readonly IMap map;
        private readonly Server.Game game;

        public MapToMapMovementCommandHandler(IMap map, Server.Game game)
        {
            this.map = map;
            this.game = game;
        }

        public void Execute(MapToMapMovementCommand command)
        {
            var fromTile = map[command.FromLocation];
            var toTile = map[command.ToLocation];
            var thing = command.Thing;

            var fromStackPosition = fromTile.GetStackPositionOfThing(thing);

            map.MoveThing(ref thing, command.ToLocation, 1);

            var toDirection = command.FromLocation.DirectionTo(command.ToLocation, true);

            MoveCreatures(fromStackPosition, toDirection,
            command.FromLocation, command.ToLocation,
            command.Thing, fromTile);

        }

        private void MoveCreatures(byte fromStackPosition, Direction toDirection, Location fromLocation,
        Location toLocation, IThing thing, ITile fromTile)
        { //todo: performance issues
            var outgoingPackets = new Queue<IOutgoingPacket>();

            var spectators = new HashSet<uint>();
            foreach (var spectator in map.GetCreaturesAtPositionZone(fromLocation))
            {
                spectators.Add(spectator);
            }
            foreach (var spectator in map.GetCreaturesAtPositionZone(toLocation))
            {
                spectators.Add(spectator);
            }

            var player = thing as IPlayer;


            foreach (var spectatorId in spectators)
            {
                var spectatorConnnection = game.Connections[spectatorId];
                var spectator = game.CreatureInstances[spectatorId];

                if (spectatorId == player.CreatureId)
                {
                    outgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation, fromStackPosition));
                    outgoingPackets.Enqueue(new MapPartialDescriptionPacket(thing, fromLocation, toLocation, toDirection, map));
                    spectatorConnnection.Send(outgoingPackets);
                    continue;
                }

                if (spectator.CanSee(fromLocation) && spectator.CanSee(toLocation))
                {
                    outgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation, fromStackPosition));
                    spectatorConnnection.Send(outgoingPackets, true);
                    continue;
                }

                if (spectator.CanSee(fromLocation)) //spectator can see old position but not the new
                {
                    //happens when player leaves spectator'ss view area
                    outgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile, fromStackPosition));
                    spectatorConnnection.Send(outgoingPackets, true);

                    continue;
                }

                if (spectator.CanSee(toLocation)) //spectator can't see old position but the new
                {
                    //happens when player enters spectator's view area
                    outgoingPackets.Enqueue(new AddAtStackPositionPacket(player));
                    outgoingPackets.Enqueue(new AddCreaturePacket((IPlayer)spectator, player));
                    spectatorConnnection.Send(outgoingPackets, true);
                    continue;
                }

            }
        }
    }
}
