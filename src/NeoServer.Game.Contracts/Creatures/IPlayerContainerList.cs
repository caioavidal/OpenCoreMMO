using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void RemoveItemFromOpenedContainer(IPlayer player, byte containerId, byte slotIndex, IItem item);
    public delegate void AddItemOnOpenedContainer(IPlayer player, byte containerId, IItem item);
    public delegate void UpdateItemOnOpenedContainer(IPlayer player, byte containerId, byte slotIndex, IItem item, sbyte amount);

    public interface IPlayerContainerList
    {
        Items.Types.IContainer this[byte id] { get; }

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
