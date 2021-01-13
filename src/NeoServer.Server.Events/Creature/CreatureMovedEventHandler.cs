using NeoServer.Game.Common.Location;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;

namespace NeoServer.Server.Events
{
    public class CreatureMovedEventHandler
    {
        private readonly Game game;

        public CreatureMovedEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IWalkableCreature creature, ICylinder cylinder)
        {
            cylinder.ThrowIfNull();
            cylinder.TileSpectators.ThrowIfNull();
            creature.ThrowIfNull();

            var toTile = cylinder.ToTile;
            var fromTile = cylinder.FromTile;
            toTile.ThrowIfNull();
            fromTile.ThrowIfNull();

            var toDirection = fromTile.Location.DirectionTo(toTile.Location, true);

            MoveCreature(toDirection, creature, cylinder);
        }

        public void MoveCreature(Direction toDirection, IWalkableCreature creature, ICylinder cylinder)
        {
            var fromLocation = cylinder.FromTile.Location;
            var toLocation = cylinder.ToTile.Location;
            var fromTile = cylinder.FromTile;
            var toTile = cylinder.ToTile;

            if (creature is IMonster && creature.IsInvisible) return;

            if (creature is ICreature)
            {
                creature.SetDirection(toDirection);
            }

            foreach (var cylinderSpectator in cylinder.TileSpectators)
            {
                var spectator = cylinderSpectator.Spectator;

                if (spectator is not IPlayer player) continue;
                if (!cylinderSpectator.Spectator.CanSee(creature.Location)) continue;

                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection)) continue;

                if (spectator.CreatureId == creature.CreatureId) //myself
                {

                    if (fromLocation.Z != toLocation.Z)
                    {
                        connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile, cylinderSpectator.FromStackPosition));
                        connection.OutgoingPackets.Enqueue(new MapDescriptionPacket(player, game.Map));
                    }
                    else if (cylinder.IsTeleport)
                    {
                        connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile, cylinderSpectator.FromStackPosition));
                        connection.OutgoingPackets.Enqueue(new MapDescriptionPacket(player, game.Map));
                    }
                    else
                    {
                        connection.OutgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation, cylinderSpectator.FromStackPosition));
                        connection.OutgoingPackets.Enqueue(new MapPartialDescriptionPacket(creature, fromLocation, toLocation, toDirection, game.Map));
                    }
                    connection.Send();
                    continue;
                }

                if (spectator.CanSee(creature) && spectator.CanSee(fromLocation) && spectator.CanSee(toLocation)) //spectator can see old and new location
                {
                    if (fromLocation.Z != toLocation.Z)
                    {
                        connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile, cylinderSpectator.FromStackPosition));
                        connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(creature, cylinderSpectator.ToStackPosition));

                        connection.OutgoingPackets.Enqueue(new AddCreaturePacket((IPlayer)spectator, creature));
                    }
                    else
                    {
                        connection.OutgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation, cylinderSpectator.FromStackPosition));
                    }
                    connection.Send();

                    continue;
                }

                if (spectator.CanSee(creature) && spectator.CanSee(fromLocation)) //spectator can see old position but not the new
                {
                    //happens when player leaves spectator's view area
                    connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile, cylinderSpectator.FromStackPosition));
                    connection.Send();

                    continue;
                }

                if (spectator.CanSee(creature) && spectator.CanSee(toLocation)) //spectator can't see old position but the new
                {
                    //happens when player enters spectator's view area
                    connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(creature, cylinderSpectator.ToStackPosition));
                    connection.OutgoingPackets.Enqueue(new AddCreaturePacket((IPlayer)spectator, creature));
                    connection.Send();
                    continue;
                }

            }
        }
    }
}
