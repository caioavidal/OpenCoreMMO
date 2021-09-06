using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface ITransformable
    {
        void Transform();
        Func<IItemType> OnEquipItem { get; }
        Func<IItemType> OnDequipItem { get; }
    }
}
