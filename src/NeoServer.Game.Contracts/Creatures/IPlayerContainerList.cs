using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void RemoveItemFromOpenedContainer(IPlayer player, byte containerId, byte slotIndex, IItem item);
    public delegate void AddItemOnOpenedContainer(IPlayer player, byte containerId, IItem item);
    public delegate void UpdateItemOnOpenedContainer(IPlayer player, byte containerId, byte slotIndex, IItem item);
    
    public interface IPlayerContainerList
    {
        IContainerItem this[byte id] { get; }

        RemoveItemFromOpenedContainer RemoveItemAction { get; set; }
        AddItemOnOpenedContainer AddItemAction { get; set; }
        UpdateItemOnOpenedContainer UpdateItemAction { get; set; }

        event ClosedContainer OnClosedContainer;
        event OpenedContainer OnOpenedContainer;

        

        void CloseContainer(byte containerId);
        void GoBackContainer(byte containerId);
        void MoveItemBetweenContainers(Location fromLocation, Location toLocation, byte count = 1);
        void OpenContainerAt(Location location, byte containerLevel);
    }
}
