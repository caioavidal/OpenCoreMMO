using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Contracts.Items
{
    public delegate void Transform(IItemType before, IItemType now);
    public interface ITransformable
    {
        void TransformOnEquip();
        void TransformOnDequip();
        Func<IItemType> TransformEquipItem { get; }
        Func<IItemType> TransformDequipItem { get; }
        event Transform OnTransformed;
    }
}
