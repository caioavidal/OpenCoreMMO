using System.Collections.Generic;
using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Events;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class ThingMovedOnFloorEventHandler
    {
        private readonly Game game;

        public ThingMovedOnFloorEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IThing thing, ITile fromTile, ITile toTile, byte fromStackPosition)
        {
            var toDirection = fromTile.Location.DirectionTo(toTile.Location, true);

            MoveThing(fromStackPosition, toDirection, fromTile.Location, toTile.Location, thing, fromTile);
        }


        private void MoveThing(byte fromStackPosition, Direction toDirection, Location fromLocation, Location toLocation, IThing thing, ITile fromTile)
        { //todo: performance issues
            var outgoingPackets = new Queue<IOutgoingPacket>();

            //  var spectators = game.Map.GetCreaturesAtPositionZone(fromLocation, toLocation);
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
                IConnection connection;
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out connection))
                {
                    continue;
                }

                var spectator = game.CreatureManager.GetCreature(spectatorId);

                if (spectatorId == player.CreatureId) //myself
                {
                    if(fromLocation.Z != toLocation.Z)
                    {
                        connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile, fromStackPosition));
                        connection.OutgoingPackets.Enqueue(new MapDescriptionPacket(player, game.Map));
                    }
                    else
                    {
                        connection.OutgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation, fromStackPosition));
                        connection.OutgoingPackets.Enqueue(new MapPartialDescriptionPacket(thing, fromLocation, toLocation, toDirection, game.Map));
                    }
                    connection.Send();
                    continue;
                }

                if (spectator.CanSee(fromLocation) && spectator.CanSee(toLocation))
                {
                    outgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation, fromStackPosition));
                    connection.Send(outgoingPackets);
                    continue;
                }

                if (spectator.CanSee(fromLocation)) //spectator can see old position but not the new
                {
                    //happens when player leaves spectator's view area
                    outgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile, fromStackPosition));
                    connection.Send(outgoingPackets);

                    continue;
                }

                if (spectator.CanSee(toLocation)) //spectator can't see old position but the new
                {
                    //happens when player enters spectator's view area
                    outgoingPackets.Enqueue(new AddAtStackPositionPacket(player));
                    outgoingPackets.Enqueue(new AddCreaturePacket((IPlayer)spectator, player));
                    connection.Send(outgoingPackets);
                    continue;
                }

            }
        }
    }
}
