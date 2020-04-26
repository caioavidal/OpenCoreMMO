using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IContainerItem:IItem
    {
        IItem[] Items { get; }
        byte Capacity { get; }
        bool HasParent { get; }
        byte SlotsUsed { get; }
        byte Id { get; set; }

        bool GetContainerAt(byte index, out IContainerItem container);
        void SetAsOpened();
        void SetParent(IContainerItem container);
        bool TryAddItem(IItem item);
    }
}
