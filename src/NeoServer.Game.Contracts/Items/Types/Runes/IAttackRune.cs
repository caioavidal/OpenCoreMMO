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
    public interface IAttackRune: IUseableOnCreature, IUseableOnItem, IRune
    {
        DamageType DamageType { get; }
        ShootType ShootType { get; }
        bool NeedTarget { get; }
    }
}
