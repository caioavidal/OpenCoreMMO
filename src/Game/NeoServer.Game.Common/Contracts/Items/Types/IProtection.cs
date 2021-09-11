using NeoServer.Game.Common.Combat.Structs;

namespace NeoServer.Game.Common.Contracts.Items.Types
{
    public interface IProtection
    {
        void Protect(ref CombatDamage damage);
    }
}
