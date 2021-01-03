using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Contracts.Combat
{
    public interface IMeleeCombatAttack
    {
        bool TryAttack(ICombatActor actor, ICombatActor enemy, DamageType damageType, int minDamage, int maxDamage, out CombatDamage damage);
    }
}
