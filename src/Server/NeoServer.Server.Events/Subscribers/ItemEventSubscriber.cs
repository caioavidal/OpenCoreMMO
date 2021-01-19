using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Useables;

namespace NeoServer.Server.Events
{
    public class ItemEventSubscriber: IItemEventSubscriber
    {
        private readonly ItemUsedOnTileEventHandler itemUsedOnTileEventHandler;

        public ItemEventSubscriber(ItemUsedOnTileEventHandler itemUsedOnTileEventHandler)
        {
            this.itemUsedOnTileEventHandler = itemUsedOnTileEventHandler;
        }

        public void Subscribe(IItem item)
        {
            if(item is IUseableOnTile useableOnTile)
            {
                useableOnTile.OnUsedOnTile += itemUsedOnTileEventHandler.Execute;
            }
        }

  
        public void Unsubscribe(IItem item)
        {
            if (item is IUseableOnTile useableOnTile)
            {
                useableOnTile.OnUsedOnTile -= itemUsedOnTileEventHandler.Execute;
            }
        }
    }
}