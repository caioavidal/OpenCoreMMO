﻿using NeoServer.Game.Combat.Attacks.Legacy;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Extensions.Spells.Attack;

public class IceStrike : AttackSpell
{
    public override DamageType DamageType => DamageType.Ice;
    public override CombatAttack CombatAttack => new DistanceCombatAttack(Range, ShootType.Ice);
    public override byte Range => 5;

    public override MinMax CalculateDamage(ICombatActor actor)
    {
        return new MinMax(5, 100);
    }
}