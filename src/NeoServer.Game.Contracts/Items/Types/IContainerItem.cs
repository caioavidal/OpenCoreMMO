using NeoServer.Game.Enums;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Items.Types
{
    public delegate void RemoveItem(byte slotIndex, IItem item);
    public delegate void AddItem(IItem item);
    public delegate void UpdateItem(byte slotIndex, IItem item, sbyte amount);

    public interface IPickupableContainer : IContainer, IPickupable
    {
        new float Weight { get; }
    }
    public interface IContainer : IItem, IInventoryItem
    {
        IItem this[int index] { get; }

        List<IItem> Items { get; }
        byte Capacity { get; }
        bool HasParent { get; }
        byte SlotsUsed { get; }
        IThing Parent { get; }
        bool IsFull { get; }

        event RemoveItem OnItemRemoved;
        event AddItem OnItemAdded;
        event UpdateItem OnItemUpdated;

        bool GetContainerAt(byte index, out IContainer container);
        Result MoveItem(byte fromSlotIndex, byte toSlotIndex);
        void MoveItem(byte fromSlotIndex, byte toSlotIndex, byte amount = 1);
        IItem RemoveItem(byte slotIndex);
        IItem RemoveItem(byte slotIndex, byte amount);
        void SetParent(IThing thing);
        Result TryAddItem(IItem item, byte slot);
        Result TryAddItem(IItem item, byte? slot = null);


    }
}
