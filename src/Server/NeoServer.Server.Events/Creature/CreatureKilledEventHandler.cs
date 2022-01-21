using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Networking.Packets.Outgoing.Login;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureKilledEventHandler
    {
        private readonly IGameServer game;
        private readonly IItemFactory itemFactory;
        private readonly IMap map;

        public CreatureKilledEventHandler(IMap map, IGameServer game, IItemFactory itemFactory)
        {
            this.map = map;
            this.game = game;
            this.itemFactory = itemFactory;
        }

        public void Execute(ICombatActor creature, IThing by, ILoot loot)
        {
            game.Scheduler.AddEvent(new SchedulerEvent(200, () =>
            {
                var tile = creature.Tile;

                var thing = creature as IThing;

                //send packets to killed player
                if (creature is IPlayer killedPlayer &&
                    game.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection))
                {
                    connection.OutgoingPackets.Enqueue(new ReLoginWindowOutgoingPacket());
                    connection.Send();
                }
            }));

            if (creature is IMonster monster && !monster.IsSummon)
                game.CreatureManager.AddKilledMonsters(creature as IMonster);
        }
    }
}