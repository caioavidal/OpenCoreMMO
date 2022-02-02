using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Server.Events.Items;

namespace NeoServer.Server.Events.Subscribers;

public class ItemEventSubscriber : IItemEventSubscriber
{
    private readonly ItemUsedOnTileEventHandler _itemUsedOnTileEventHandler;
    private readonly ItemStartedDecayingEventHandler _itemStartedDecayingEventHandler;

    public ItemEventSubscriber(ItemUsedOnTileEventHandler itemUsedOnTileEventHandler,
        ItemStartedDecayingEventHandler itemStartedDecayingEventHandler)
    {
        _itemUsedOnTileEventHandler = itemUsedOnTileEventHandler;
        _itemStartedDecayingEventHandler = itemStartedDecayingEventHandler;
    }

    public void Subscribe(IItem item)
    {
        if (item is IUsableOnTile useableOnTile) useableOnTile.OnUsedOnTile += _itemUsedOnTileEventHandler.Execute;
        if (item.HasDecayBehavior && item is IHasDecay decayItem) decayItem.Decayable.OnStarted += _itemStartedDecayingEventHandler.Execute;
    }

    public void Unsubscribe(IItem item)
    {
        if (item is IUsableOnTile useableOnTile) useableOnTile.OnUsedOnTile -= _itemUsedOnTileEventHandler.Execute;
        if (item.HasDecayBehavior && item is IHasDecay decayItem) decayItem.Decayable.OnStarted -= _itemStartedDecayingEventHandler.Execute;
    }
}