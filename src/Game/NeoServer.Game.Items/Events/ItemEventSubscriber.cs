using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Runes;

namespace NeoServer.Game.Items.Events
{
    public class ItemEventSubscriber : IItemEventSubscriber, IGameEventSubscriber
    {
        private readonly ItemUsedEventHandler itemUsedEventHandler;
        private readonly FieldRuneUsedEventHandler fieldRuneUsedEventHandler;
        public ItemEventSubscriber(ItemUsedEventHandler itemUsedEventHandler, FieldRuneUsedEventHandler fieldRuneUsedEventHandler)
        {
            this.itemUsedEventHandler = itemUsedEventHandler;
            this.fieldRuneUsedEventHandler = fieldRuneUsedEventHandler;
        }

        public void Subscribe(IItem item)
        {
            if (item is IConsumable consumable)
            {
                consumable.OnUsed += itemUsedEventHandler.Execute;
            }
            if(item is IFieldRune fieldRune)
            {
                fieldRune.OnUsedOnTile += fieldRuneUsedEventHandler.Execute;
            }
        }

        public void Unsubscribe(IItem item)
        {
            if (item is IConsumable consumable)
            {
                consumable.OnUsed -= itemUsedEventHandler.Execute;
            }
            if (item is IFieldRune fieldRune)
            {
                fieldRune.OnUsedOnTile -= fieldRuneUsedEventHandler.Execute;
            }
        }
    }
}
