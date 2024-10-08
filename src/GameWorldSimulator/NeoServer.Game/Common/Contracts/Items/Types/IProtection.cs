using NeoServer.Game.Common.Combat;

namespace NeoServer.Game.Common.Contracts.Items.Types;

public interface IProtection
{
    bool Protect(ref CombatDamage damage);
}