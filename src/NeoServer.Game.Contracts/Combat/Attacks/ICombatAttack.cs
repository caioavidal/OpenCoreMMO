using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Combat.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat.Attacks
{
    public interface ICombatAttack
    {
        bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatAttackType combatType);
    }
}
