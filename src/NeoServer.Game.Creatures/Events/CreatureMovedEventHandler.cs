using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Creatures.Events
{
    public class CreatureMovedEventHandler : IGameEventHandler
    {
        private readonly IMap map;

        public CreatureMovedEventHandler(IMap map)
        {
            this.map = map;
        }

        public void Execute(ICreature creature)
        {
            foreach (var spectator in map.GetCreaturesAtPositionZone(creature.Location))
            {
                if (creature == spectator) continue;

                if (spectator is ICombatActor actor) actor.SetAsEnemy(creature);
                if (creature is ICombatActor a) a.SetAsEnemy(spectator);
            }
        }
    }
}
