﻿using System.Linq;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Combat.Attacks;

public class AreaCombatAttack : CombatAttack
{
    public AreaCombatAttack(byte radius)
    {
        Radius = radius;
    }

    public byte Radius { get; set; }

    public override bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option,
        out CombatAttackResult combatResult)
    {
        combatResult = new CombatAttackResult();

        if (CalculateAttack(actor, enemy, option, out var damage))
        {
            combatResult.DamageType = option.DamageType;
            var location = enemy?.Location ?? actor.Location;
            
            combatResult.SetArea(ExplosionEffect.Create(location, Radius).ToArray());
            actor.PropagateAttack(combatResult.Area, damage);
            return true;
        }

        return false;
    }
}