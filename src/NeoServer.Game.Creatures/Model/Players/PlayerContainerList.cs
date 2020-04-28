using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Players
{
    public class PlayerContainerList : IPlayerContainerList
    {
        public event ClosedContainer OnClosedContainer;
        public event OpenedContainer OnOpenedContainer;

        public RemoveItemFromOpenedContainer RemoveItemAction { get; set; }
        public AddItemOnOpenedContainer AddItemAction { get; set; }


        private readonly IPlayer player;
        public PlayerContainerList(IPlayer player)
        {
            this.player = player;

        }

        public IContainerItem this[byte id]=> openedContainers.ContainsKey(id) ? openedContainers[id]?.Container : null;

        private Dictionary<byte, PlayerContainer> openedContainers = new Dictionary<byte, PlayerContainer>();

        public void GoBackContainer(byte containerId)
        {
            if (openedContainers.TryGetValue(containerId, out var playerContainer))
            {
                var parentContainer = playerContainer.Container.Parent;
                openedContainers[containerId].DetachContainerEvents();

                openedContainers[containerId] = new PlayerContainer(parentContainer, player);
                OnOpenedContainer?.Invoke(player, containerId, parentContainer);

            }

        }
        public void OpenContainerAt(Location location, byte containerLevel)
        {
            PlayerContainer playerContainer = null;

            if (location.Slot == Slot.Backpack)
            {
                playerContainer = new PlayerContainer(player.Inventory.BackpackSlot, player);

            }
            else if (location.Type == LocationType.Container)
            {
                var parentContainer = openedContainers[location.ContainerId]?.Container;
                parentContainer.GetContainerAt((byte)location.ContainerPosition, out var container);
                playerContainer = new PlayerContainer(container, player);
            }

            if (openedContainers.ContainsValue(playerContainer)) //if container is already opened
            {
                CloseContainer(playerContainer.Id); //just close container and return
                return;
            }

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


            playerContainer.AttachActions(RemoveItemAction, AddItemAction);
            playerContainer.AttachContainerEvent();

            OnOpenedContainer?.Invoke(player, playerContainer.Id, playerContainer.Container);
            return;

        }

        public void MoveItemBetweenContainers(Location fromLocation, Location toLocation)
        {
            var fromContainer = openedContainers[fromLocation.ContainerId];
            var toContainer = openedContainers[toLocation.ContainerId];

            if (fromContainer.Id == toContainer.Id)
            {
                fromContainer.Container.MoveItemToChild((byte)fromLocation.ContainerPosition, (byte)toLocation.ContainerPosition);
                return;
            }

            if(fromContainer.Container[fromLocation.ContainerPosition] == toContainer.Container)
            {
                return ;
            }

            var item = fromContainer.Container.RemoveItem((byte)fromLocation.ContainerPosition);

            toContainer.Container.TryAddItem(item);

            if (item is IContainerItem container)
            {
                container.SetParent(toContainer.Container);
            }
        }

        public void CloseContainer(byte containerId)
        {
            if (openedContainers.Remove(containerId, out var playerContainer))
            {
                playerContainer.DetachContainerEvents();
                OnClosedContainer?.Invoke(player, containerId);
            }
        }

       
    }

}
