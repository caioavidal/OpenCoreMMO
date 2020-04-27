using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public class ContainerItem : MoveableItem, IContainerItem, IItem
    {
        public ContainerItem(IItemType type) : base(type)
        {
            Capacity = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Capacity);

            Items = new IItem[Capacity];
        }
        private byte slotIndex;

        public byte SlotsUsed => slotIndex;
        public IContainerItem Parent { get; private set; }
        public bool HasParent => Parent != null;
        public byte Capacity { get; }
        public IItem[] Items { get; }
        public byte Id { get; set; }

        
        public void SetParent(IContainerItem container)
        {
            Parent = container;
        }

        public static bool IsApplicable(IItemType type) => type.Group == Enums.ItemGroup.GroundContainer || 
            type.Attributes.GetAttribute(Enums.ItemAttribute.Type)?.ToLower() == "container";

        public bool GetContainerAt(byte index, out IContainerItem container)
        {
            container = null;
            if (Items[index] is IContainerItem)
            {
                container = Items[index] as IContainerItem;
                return true;
            }
            
            return false;
        }
        public bool TryAddItem(IItem item)
        {
            if (slotIndex + 1 >= Capacity)
            {
                return false;
            }
            Items[slotIndex++] = item;
            return true;
        }
    }
}
