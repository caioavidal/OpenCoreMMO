using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Creatures.Events
{
    public class CreatureMovedEventHandler : IGameEventHandler
    {
        public void Execute(ICreature creature, Location fromLocation, Location toLocation, ICylinderSpectator[] spectators)
        {
            foreach (var cylinderSpectator in spectators) 
            {
                var spectator = cylinderSpectator.Spectator;
                if (creature == spectator) continue;
                
                if (spectator is ICombatActor actor) actor.SetAsEnemy(creature);
                if (creature is ICombatActor a) a.SetAsEnemy(spectator);
            }
        }
    }
}
