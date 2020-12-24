using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Game.Items.Events
{
    public class ItemEventSubscriber : IItemEventSubscriber, IGameEventSubscriber
    {
        private readonly ItemUsedEventHandler itemUsedEventHandler;

        public ItemEventSubscriber(ItemUsedEventHandler itemUsedEventHandler)
        {
            this.itemUsedEventHandler = itemUsedEventHandler;
        }

        public void Subscribe(IItem item)
        {
            if (item is IConsumable consumable)
            {
                consumable.OnUsed += itemUsedEventHandler.Execute;
            }
        }

        public void Unsubscribe(IItem item)
        {
            if (item is IConsumable consumable)
            {
                consumable.OnUsed -= itemUsedEventHandler.Execute;
            }
        }
    }
}
