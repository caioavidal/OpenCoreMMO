using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Contracts.Combat.Attacks
{
    public interface ICombatAttack
    {
        bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackValue option, out CombatAttackType combatType);
    }
}
