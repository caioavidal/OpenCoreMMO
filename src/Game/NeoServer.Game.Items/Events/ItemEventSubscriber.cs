using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Types.Useables;

namespace NeoServer.Game.Items.Events
{
    public class ItemEventSubscriber : IItemEventSubscriber, IGameEventSubscriber
    {
        private readonly FieldRuneUsedEventHandler fieldRuneUsedEventHandler;
        private readonly ItemUsedEventHandler itemUsedEventHandler;

        public ItemEventSubscriber(ItemUsedEventHandler itemUsedEventHandler,
            FieldRuneUsedEventHandler fieldRuneUsedEventHandler)
        {
            this.itemUsedEventHandler = itemUsedEventHandler;
            this.fieldRuneUsedEventHandler = fieldRuneUsedEventHandler;
        }

        public void Subscribe(IItem item)
        {
            if (item is IConsumable consumable) consumable.OnUsed += itemUsedEventHandler.Execute;
            if (item is IFieldRune fieldRune) fieldRune.OnUsedOnTile += fieldRuneUsedEventHandler.Execute;
        }

        public void Unsubscribe(IItem item)
        {
            if (item is IConsumable consumable) consumable.OnUsed -= itemUsedEventHandler.Execute;
            if (item is IFieldRune fieldRune) fieldRune.OnUsedOnTile -= fieldRuneUsedEventHandler.Execute;
        }
    }
}