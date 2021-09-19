using System;

namespace NeoServer.Game.Common.Contracts.Items
{
    public delegate void Transform(IItemType before, IItemType now);
    public interface ITransformable
    {
        void TransformOnEquip();
        void TransformOnDequip();
        IItemType TransformEquipItem { get; }
        IItemType TransformDequipItem { get; }
        event Transform OnTransformed;
    }
}
