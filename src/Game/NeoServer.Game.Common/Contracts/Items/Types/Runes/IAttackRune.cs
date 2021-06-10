using NeoServer.Game.Common.Contracts.Items.Types.Useables;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Runes
{
    public interface IAttackRune : IUseableAttackOnCreature, IUseableAttackOnTile, IRune
    {
        /// <summary>
        ///     Rune's Damage Type
        /// </summary>
        DamageType DamageType { get; }

        /// <summary>
        ///     Shoot Type
        /// </summary>
        ShootType ShootType { get; }

        /// <summary>
        ///     Damage Effect
        /// </summary>
        EffectT Effect { get; }

        /// <summary>
        ///     True when damage is not in area
        /// </summary>
        bool NeedTarget { get; }
    }
}