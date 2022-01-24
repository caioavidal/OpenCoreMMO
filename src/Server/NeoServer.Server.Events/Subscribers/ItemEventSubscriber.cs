using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Server.Events.Items;

namespace NeoServer.Server.Events.Subscribers;

public class ItemEventSubscriber : IItemEventSubscriber
{
    private readonly ItemUsedOnTileEventHandler itemUsedOnTileEventHandler;

    public ItemEventSubscriber(ItemUsedOnTileEventHandler itemUsedOnTileEventHandler)
    {
        this.itemUsedOnTileEventHandler = itemUsedOnTileEventHandler;
    }

    public void Subscribe(IItem item)
    {
        if (item is IUsableOnTile useableOnTile) useableOnTile.OnUsedOnTile += itemUsedOnTileEventHandler.Execute;
    }

    public void Unsubscribe(IItem item)
    {
        if (item is IUsableOnTile useableOnTile) useableOnTile.OnUsedOnTile -= itemUsedOnTileEventHandler.Execute;
    }
}