using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class ThingRemovedFromTileEventHandler
    {
        private readonly Game game;

        public ThingRemovedFromTileEventHandler(IMap map, Game game)
        {
            this.game = game;
        }
        public void Execute(IThing thing, ICylinder cylinder)
        {

            cylinder.ThrowIfNull();
            cylinder.TileSpectators.ThrowIfNull();
            thing.ThrowIfNull();

            var tile = cylinder.FromTile;
            tile.ThrowIfNull();

            foreach (var spectator in cylinder.TileSpectators)
            {
                var creature = spectator.Spectator;

                if (!game.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection))
                {
                    continue;
                }

                if (creature is not IPlayer player) continue;

                if (player.IsDead && thing != player)
                {
                    continue;
                }

                var stackPosition = spectator.FromStackPosition;

                if ((thing is IPlayer p && !p.IsDead) || (thing is IMonster monsterRemoved && monsterRemoved.IsSummon))
                {
                    connection.OutgoingPackets.Enqueue(new MagicEffectPacket(tile.Location, EffectT.Puff));
                }

                connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(tile, stackPosition));

                connection.Send();
            }

        }
    }
}
