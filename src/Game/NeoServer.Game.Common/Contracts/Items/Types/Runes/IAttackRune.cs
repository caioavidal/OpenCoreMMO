using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Runes;

public interface IAttackRune : IUsableAttackOnCreature, IUsableAttackOnTile, IRune
{
    /// <summary>
    ///     Rune's Damage Type
    /// </summary>
    DamageType DamageType { get; }

    /// <summary>
    ///     Damage Effect
    /// </summary>
    new EffectT Effect { get; }
}