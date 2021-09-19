using System;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Items.Items.Attributes
{
    public class Transformable : ITransformable
    {
        private IItemType _itemType;

        public Transformable(IItemType itemType)
        {
            _itemType = itemType;
        }

        public void TransformOnEquip()
        {
            if (TransformEquipItem is not { } itemType) return;
            var before = _itemType;
            _itemType = itemType;

            OnTransformed?.Invoke(before, itemType);
        }


        public void TransformOnDequip()
        {
            if (TransformDequipItem is not { } itemType) return;

            var before = _itemType;
            _itemType = itemType;

            OnTransformed?.Invoke(before, itemType);
        }

        public IItemType TransformEquipItem { get; init; }
        public IItemType TransformDequipItem { get; init; }
        public event Transform OnTransformed;
    }
}