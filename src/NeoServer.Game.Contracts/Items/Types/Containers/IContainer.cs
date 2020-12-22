using NeoServer.Game.Common;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Items.Types
{
    public delegate void RemoveItem(byte slotIndex, IItem item);
    public delegate void AddItem(IItem item);
    public delegate void UpdateItem(byte slotIndex, IItem item, sbyte amount);
    public delegate void DeleteContainer(IContainer container);
    public delegate void Move(IContainer container);

    public interface IContainer : IItem, IInventoryItem, IStore
    {
        IItem this[int index] { get; }
        /// <summary>
        /// Items on container
        /// </summary>
        List<IItem> Items { get; }
        /// <summary>
        /// Container's capacity. It indicates how many slots are available on container
        /// </summary>
        byte Capacity { get; }
        /// <summary>
        /// Indicates if container has any parent
        /// </summary>
        bool HasParent { get; }
        byte SlotsUsed { get; }
        IThing Parent { get; }
        bool IsFull { get; }
        bool HasItems { get; }
        IThing Root { get; }

        event RemoveItem OnItemRemoved;
        event AddItem OnItemAdded;
        event UpdateItem OnItemUpdated;
        event Move OnContainerMoved;

        bool GetContainerAt(byte index, out IContainer container);
        //Result MoveItem(byte fromSlotIndex, byte toSlotIndex, byte amount = 1);
        void SetParent(IThing thing);
        //Result TryAddItem(IItem item, byte? slot = null);
        void Clear();

        string IThing.InspectionText => $"{Metadata.Article} {Name} (Vol:{Capacity})";
    }
}
