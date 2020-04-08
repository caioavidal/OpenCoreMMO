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
            var fromTile = game.Map[fromLocation];
            var toTile = game.Map[toLocation];

            var fromStackPosition = fromTile.GetStackPositionOfThing(thing);

            game.Map.MoveThing(ref thing, toLocation, 1);

            var toDirection = fromLocation.DirectionTo(toLocation, true);

            MoveCreatures(fromStackPosition, toDirection,
            fromLocation, toLocation,
            thing, fromTile);
        }

        private void MoveCreatures(byte fromStackPosition, Direction toDirection, Location fromLocation,
     Location toLocation, IThing thing, ITile fromTile)
        { //todo: performance issues
            var outgoingPackets = new Queue<IOutgoingPacket>();

            var spectators = new HashSet<uint>();
            foreach (var spectator in game.Map.GetCreaturesAtPositionZone(fromLocation))
            {
                spectators.Add(spectator);
            }
            foreach (var spectator in game.Map.GetCreaturesAtPositionZone(toLocation))
            {
                spectators.Add(spectator);
            }

            var player = thing as IPlayer;


            foreach (var spectatorId in spectators)
            {
                var spectatorConnnection = game.CreatureManager.GetPlayerConnection(spectatorId);
                var spectator = game.CreatureManager.GetCreature(spectatorId);

                if (spectatorId == player.CreatureId)
                {
                    outgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation, fromStackPosition));
                    outgoingPackets.Enqueue(new MapPartialDescriptionPacket(thing, fromLocation, toLocation, toDirection, game.Map));
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
