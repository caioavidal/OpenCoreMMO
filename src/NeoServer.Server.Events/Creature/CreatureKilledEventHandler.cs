using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Items;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureKilledEventHandler
    {
        private readonly IMap map;
        private readonly Game game;
        private readonly IItemFactory itemFactory;
        public CreatureKilledEventHandler(IMap map, Game game, IItemFactory itemFactory)
        {
            this.map = map;
            this.game = game;
            this.itemFactory = itemFactory;
        }
        public void Execute(ICombatActor creature)
        {
            game.Scheduler.AddEvent(new SchedulerEvent(200, () =>
            {
                var tile = creature.Tile;

                var thing = creature as IMoveableThing;

                map.RemoveThing(ref thing, tile);

                var corpse = itemFactory.Create(creature.Corpse, creature.Location, null) as IMoveableThing;
                map.AddItem(ref corpse, tile);

                //send packets to killed player
                if (creature is IPlayer killedPlayer && game.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection))
                {
                    connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(tile, 1));
                    connection.OutgoingPackets.Enqueue(new AddTileItemPacket((IItem)corpse, 1));
                    connection.OutgoingPackets.Enqueue(new ReLoginWindowOutgoingPacket());
                    connection.Send();
                }
            }));

            if (creature is IMonster monster)
            {
                game.CreatureManager.AddKilledMonsters(creature as IMonster);
            }
        }
    }
}
