using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureWasBornEventHandler
    {
        private readonly IGameServer game;
        private readonly IMap map;

        public CreatureWasBornEventHandler(IMap map, IGameServer game)
        {
            this.map = map;
            this.game = game;
        }

        public void Execute(IMonster creature, Location location)
        {
            map.PlaceCreature(creature);
        }
    }
}