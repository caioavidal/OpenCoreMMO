using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat
{
    public interface IMeleeCombatAttack
    {
        bool TryAttack(ICombatActor actor, ICombatActor enemy, DamageType damageType, int minDamage, int maxDamage, out CombatDamage damage);
    }
}
