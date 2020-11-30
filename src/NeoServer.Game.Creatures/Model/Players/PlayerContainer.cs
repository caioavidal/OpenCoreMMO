using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Model.Players.Contracts;
using System;

namespace NeoServer.Game.Creatures.Model.Players
{
    internal class PlayerContainer : IEquatable<PlayerContainer>
    {
        public IPlayer Player { get; }
        public PlayerContainer(IContainer container, IPlayer player)
        {
            Container = container;
            Player = player;
        }

        public byte Id { get; set; }

        public IContainer Container { get; }

        public RemoveItemFromOpenedContainer RemoveItem { get; private set; }
        public AddItemOnOpenedContainer AddItem { get; private set; }
        public UpdateItemOnOpenedContainer UpdateItem { get; private set; }
        private bool eventsAttached;

        public void ItemAdded(IItem item)
        {
            AddItem?.Invoke(Player, Id, item);
        }
        public void ItemRemoved(byte slotIndex, IItem item)
        {
            RemoveItem?.Invoke(Player, Id, slotIndex, item);
        }
        public void ItemUpdated(byte slotIndex, IItem item, sbyte amount)
        {
            UpdateItem?.Invoke(Player, Id, slotIndex, item, amount);
        }

        public void AttachActions(RemoveItemFromOpenedContainer removeItemAction, AddItemOnOpenedContainer addItemAction, UpdateItemOnOpenedContainer updateItemAction)
        {
            if (RemoveItem == null)
            {
                RemoveItem += removeItemAction;
            }
            if (AddItem == null)
            {
                AddItem += addItemAction;
            }
            if (UpdateItem == null)
            {
                UpdateItem += updateItemAction;
            }
        }

        public void AttachContainerEvent()
        {
            if (eventsAttached)
            {
                return;
            }
            DetachContainerEvents();
            Container.OnItemAdded += ItemAdded;
            Container.OnItemRemoved += ItemRemoved;
            Container.OnItemUpdated += ItemUpdated;
            eventsAttached = true;
        }
        internal void DetachContainerEvents()
        {
            Container.OnItemRemoved -= ItemRemoved;
            Container.OnItemAdded -= ItemAdded;
            Container.OnItemUpdated -= ItemUpdated;
        }

        public bool Equals(PlayerContainer obj)
        {
            return Container == obj.Container;
        }
    }
}
