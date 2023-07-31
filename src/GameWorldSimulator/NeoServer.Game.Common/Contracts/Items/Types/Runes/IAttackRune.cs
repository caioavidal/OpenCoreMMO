using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Runes;

public interface IAttackRune : IRune
{
    /// <summary>
    ///     Rune's Damage Type
    /// </summary>
    DamageType DamageType { get; }

    /// <summary>
    ///     Damage Effect
    /// </summary>
    new EffectT Effect { get; }
    CombatAttackParams PrepareAttack(IPlayer player);
    bool Use(ICombatActor aggressor, IThing victim, IItemCombatAttack combatAttack);
    bool NeedTarget { get; }
    string Area { get; }
}