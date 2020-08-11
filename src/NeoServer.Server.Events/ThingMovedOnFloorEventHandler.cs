using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
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
        {

            var spectators = game.Map.GetCreaturesAtPositionZone(fromLocation, toLocation);

            var player = thing as IPlayer;

            player.SetDirection(toDirection);

            foreach (var spectatorId in spectators)
            {


                if (!game.CreatureManager.TryGetCreature(spectatorId, out ICreature spectator))
                {
                    continue;
                }

                if (spectator is IMonster && !spectator.Attacking)
                {
                    spectator.Attack(player);
                    continue;
                }

                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }


                if (spectatorId == player.CreatureId) //myself
                {
                    if (fromLocation.Z != toLocation.Z)
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
                    if (fromLocation.Z != toLocation.Z)
                    {
                        connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile, fromStackPosition));
                        connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(player));

                        connection.OutgoingPackets.Enqueue(new AddCreaturePacket((IPlayer)spectator, player));

                    }
                    else
                    {
                        connection.OutgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation, fromStackPosition));
                    }
                    connection.Send();

                    continue;
                }

                if (spectator.CanSee(fromLocation)) //spectator can see old position but not the new
                {
                    //happens when player leaves spectator's view area
                    connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile, fromStackPosition));
                    connection.Send();

                    continue;
                }

                if (spectator.CanSee(toLocation)) //spectator can't see old position but the new
                {
                    //happens when player enters spectator's view area
                    connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(player));
                    connection.OutgoingPackets.Enqueue(new AddCreaturePacket((IPlayer)spectator, player));
                    connection.Send();
                    continue;
                }

            }
        }
    }
}
