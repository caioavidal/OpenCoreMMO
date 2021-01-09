using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureWasBornEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public CreatureWasBornEventHandler(IMap map, Game game)
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
