using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts.Items.Types.Useables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Items.Types.Runes
{
    public interface IAttackRune: IUseableAttackOnCreature, IUseableOnItem, IRune
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
