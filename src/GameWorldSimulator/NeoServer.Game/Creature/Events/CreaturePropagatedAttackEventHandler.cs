using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;

namespace NeoServer.Game.Creature.Events;

public class CreaturePropagatedAttackEventHandler : IGameEventHandler
{
    private readonly IMap map;

    public CreaturePropagatedAttackEventHandler(IMap map)
    {
        this.map = map;
    }

    public void Execute(ICombatActor actor, CombatDamage damage, AffectedLocation[] area)
    {
        map.PropagateAttack(actor, damage, area);
    }
}