using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
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

                var thing = creature as IThing;

                //send packets to killed player
                if (creature is IPlayer killedPlayer && game.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection))
                {
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
