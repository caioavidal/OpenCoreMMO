using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types
{
    public delegate void RemoveItem(byte slotIndex, IItem item);
    public delegate void AddItem(IItem item);
    public interface IContainerItem:IItem
    {
        IItem this[int index] { get; }

        List<IItem> Items { get; }
        byte Capacity { get; }
        bool HasParent { get; }
        byte SlotsUsed { get; }
        IContainerItem Parent { get; }

        event RemoveItem OnItemRemoved;
        event AddItem OnItemAdded;

        bool GetContainerAt(byte index, out IContainerItem container);
        void MoveItemToChild(byte fromSlotIndex, byte toSlotIndex);
        IItem RemoveItem(byte slotIndex);
        void SetParent(IContainerItem container);
        bool TryAddItem(IItem item);
    }
}
