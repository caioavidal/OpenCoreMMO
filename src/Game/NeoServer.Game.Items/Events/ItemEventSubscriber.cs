using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;

namespace NeoServer.Game.Items.Events;

public class ItemEventSubscriber : IItemEventSubscriber, IGameEventSubscriber
{
    private readonly FieldRuneUsedEventHandler _fieldRuneUsedEventHandler;
    private readonly ItemTransformedEventHandler _itemTransformedEventHandler;
    private readonly ItemUsedEventHandler _itemUsedEventHandler;

    public ItemEventSubscriber(ItemUsedEventHandler itemUsedEventHandler,
        FieldRuneUsedEventHandler fieldRuneUsedEventHandler, ItemTransformedEventHandler itemTransformedEventHandler)
    {
        _itemUsedEventHandler = itemUsedEventHandler;
        _fieldRuneUsedEventHandler = fieldRuneUsedEventHandler;
        _itemTransformedEventHandler = itemTransformedEventHandler;
    }

    public void Subscribe(IItem item)
    {
        if (item is IConsumable consumable) consumable.OnUsed += _itemUsedEventHandler.Execute;
        if (item is IFieldRune fieldRune) fieldRune.OnUsedOnTile += _fieldRuneUsedEventHandler.Execute;

        if (item.IsTransformable) item.OnTransform += _itemTransformedEventHandler.Execute;
    }

    public void Unsubscribe(IItem item)
    {
        if (item is IConsumable consumable) consumable.OnUsed -= _itemUsedEventHandler.Execute;
        if (item is IFieldRune fieldRune) fieldRune.OnUsedOnTile -= _fieldRuneUsedEventHandler.Execute;

        if (item.IsTransformable) item.OnTransform -= _itemTransformedEventHandler.Execute;
    }
}