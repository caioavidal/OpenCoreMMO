using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Events
{
    public class CreaturePropagatedAttackEventHandler : IGameEventHandler
    {
        private readonly IMap map;
        public CreaturePropagatedAttackEventHandler(IMap map)
        {
            this.map = map;
        }
        public void Execute(ICombatActor actor, CombatDamage damage, Coordinate[] area)
        {
            map.PropagateAttack(actor, damage, area);
        }
    }
}
