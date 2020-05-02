using NeoServer.Game.Enums.Location;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types
{
    public delegate void RemoveItem(byte slotIndex, IItem item);
    public delegate void AddItem(IItem item);
    public delegate void UpdateItem(byte slotIndex, IItem item);
    public interface IContainer : IItem
    {
        IItem this[int index] { get; }

        List<IItem> Items { get; }
        byte Capacity { get; }
        bool HasParent { get; }
        byte SlotsUsed { get; }
        IContainer Parent { get; }
        bool IsFull { get; }

        event RemoveItem OnItemRemoved;
        event AddItem OnItemAdded;
        event UpdateItem OnItemUpdated;

        bool GetContainerAt(byte index, out IContainer container);
        bool MoveItem(byte fromSlotIndex, byte toSlotIndex, out InvalidOperation error);
        void MoveItem(byte fromSlotIndex, byte toSlotIndex, byte amount);
        IItem RemoveItem(byte slotIndex);
        IItem RemoveItem(byte slotIndex, byte amount);
        void SetParent(IContainer container);
        bool TryAddItem(IItem item, byte slot, out InvalidOperation error);
        bool TryAddItem(IItem item, byte? slot = null);
    }
}
