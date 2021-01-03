using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts.Items.Types.Useables;

namespace NeoServer.Game.Contracts.Items.Types.Runes
{
    public interface IAttackRune: IUseableAttackOnCreature, IUseableAttackOnTile, IRune
    {
        /// <summary>
        /// Rune's Damage Type 
        /// </summary>
        DamageType DamageType { get; }
        /// <summary>
        /// Shoot Type
        /// </summary>
        ShootType ShootType { get; }
        /// <summary>
        /// Damage Effect
        /// </summary>
        EffectT Effect { get; }
        /// <summary>
        /// True when damage is not in area
        /// </summary>
        bool NeedTarget { get; }
    }
}
