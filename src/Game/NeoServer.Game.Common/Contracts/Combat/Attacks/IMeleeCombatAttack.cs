using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Combat.Attacks
{
    public interface IMeleeCombatAttack
    {
        bool TryAttack(ICombatActor actor, ICombatActor enemy, DamageType damageType, int minDamage, int maxDamage,
            out CombatDamage damage);
    }
}