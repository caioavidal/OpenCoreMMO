using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Combat.Attacks;

public interface ICombatAttack
{
    bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option,
        out CombatAttackResult combatResult);
}