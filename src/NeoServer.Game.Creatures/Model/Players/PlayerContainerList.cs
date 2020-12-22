using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Creatures.Model.Players
{
    public class PlayerContainerList : IPlayerContainerList
    {
        public event ClosedContainer OnClosedContainer;
        public event OpenedContainer OnOpenedContainer;
        public RemoveItemFromOpenedContainer RemoveItemAction { get; set; }
        public AddItemOnOpenedContainer AddItemAction { get; set; }
        public UpdateItemOnOpenedContainer UpdateItemAction { get; set; }
        public MoveOpenedContainer MoveOpenedContainer { get; private set; }

        public bool HasAnyDepotOpened
        {
            get
            {
                foreach (var container in openedContainers.Values)
                {
                    if (container.Container.Root is IDepot) return true;
                }
                return false;
            }
        }
        private readonly IPlayer player;
        public PlayerContainerList(IPlayer player)
        {
            this.player = player;
            MoveOpenedContainer += CloseDistantContainer;
        }

        public void CloseDistantContainer(byte containerId, IContainer container)
        {
            if (openedContainers.Count == 0) return;

            var containerLocation = container.Root?.Location;

            if (containerLocation is null) return;

            if (containerLocation.Value.Type == LocationType.Ground &&
                containerLocation.Value.IsNextTo(player.Location) is false)
            {
                CloseContainer(containerId);
                return;
            }

            if (container.Root is IPlayer playerOwner && playerOwner != player) CloseContainer(containerId);
        }
        public void CloseDistantContainers()
        {
            if (openedContainers.Count == 0) return;
            foreach (var container in openedContainers.Values)
            {
                CloseDistantContainer(container.Id, container.Container);
            }
        }

        public IContainer this[byte id] => openedContainers.ContainsKey(id) ? openedContainers[id]?.Container : null;

        private Dictionary<byte, PlayerContainer> openedContainers = new Dictionary<byte, PlayerContainer>();

        public void GoBackContainer(byte containerId)
        {
            if (openedContainers.TryGetValue(containerId, out var playerContainer))
            {
                var parentContainer = playerContainer.Container.Parent;

                InsertOrOverrideOpenedContainer(containerId, new PlayerContainer(parentContainer as IContainer, player));

                OnOpenedContainer?.Invoke(player, containerId, parentContainer as IContainer);
            }
        }

        public void OpenContainerAt(Location location, byte containerLevel, IContainer containerToOpen = null)
        {
            PlayerContainer playerContainer = null;

            if (location.Type == LocationType.Ground)
            {
                if (!player.Location.IsNextTo(containerToOpen.Location)) return;
                playerContainer = new PlayerContainer(containerToOpen, player);
            }
            else if (location.Slot == Slot.Backpack)
            {
                playerContainer = new PlayerContainer(player.Inventory.BackpackSlot, player);

            }
            else if (location.Type == LocationType.Container)
            {
                var parentContainer = openedContainers[location.ContainerId]?.Container;
                parentContainer.GetContainerAt((byte)location.ContainerSlot, out var container);
                if (container is not IContainer) return;
                playerContainer = new PlayerContainer(container, player);
            }

            var containerAlreadyOpened = openedContainers.Values.FirstOrDefault(v => v.Equals(playerContainer))?.Id;
            if (containerAlreadyOpened.HasValue) //if container is already opened
            {
                CloseContainer(containerAlreadyOpened.Value); //just close container and return
                return;
            }

            InsertOrOverrideOpenedContainer(containerLevel, playerContainer);

            OnOpenedContainer?.Invoke(player, playerContainer.Id, playerContainer.Container);
            return;
        }

        private void InsertOrOverrideOpenedContainer(byte containerLevel, PlayerContainer playerContainer)
        {
            playerContainer.Id = containerLevel;

            if (openedContainers.ContainsKey(containerLevel)) //when opening container in the same level. Just override the opened container
            {
                openedContainers[containerLevel].DetachContainerEvents(); //detach all container events from the old container
                openedContainers[containerLevel] = playerContainer; // set the new container
            }
            else
            {
                openedContainers.TryAdd(playerContainer.Id, playerContainer);
            }

            playerContainer.AttachActions(RemoveItemAction, AddItemAction, UpdateItemAction, MoveOpenedContainer);
            playerContainer.AttachContainerEvent();
        }

        public void MoveItemBetweenContainers(Location fromLocation, Location toLocation, byte amount = 1)
        {
            var fromContainer = openedContainers[fromLocation.ContainerId];
            var toContainer = openedContainers[toLocation.ContainerId];

            var item = fromContainer.Container[fromLocation.ContainerSlot];

            if (item == toContainer.Container)
            {
                return;
            }

            var result = player.MoveThing(fromContainer.Container, toContainer.Container, item, amount, (byte)fromLocation.ContainerSlot, (byte)toLocation.ContainerSlot);
            
            if (result.IsSuccess && item is IContainer container)
            {
                container.SetParent(toContainer.Container);
            }
        }

        public void CloseContainer(byte containerId)
        {
            if (openedContainers.Remove(containerId, out var playerContainer))
            {
                playerContainer.DetachContainerEvents();
                OnClosedContainer?.Invoke(player, containerId, playerContainer.Container);
            }
        }
    }
}
