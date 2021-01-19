using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Items;
using NeoServer.OTB.Structure;

namespace NeoServer.Loaders.Items
{
    public class ItemNodeParser
    {
        /// <summary>
        /// Parses ItemNode object to IItemType
        /// </summary>
        /// <param name="itemNode"></param>
        /// <returns></returns>
        public static IItemType Parse(ItemNode itemNode)
        {

            var itemType = new ItemType();

            itemType.SetId(itemNode.ServerId);
            itemType.SetClientId(itemNode.ClientId);
            itemType.SetSpeed(itemNode.Speed);
            itemType.SetLight(new LightBlock(itemNode.LightLevel, itemNode.LightColor));
            itemType.SetWareId(itemNode.WareId);
            itemType.SetGroup((byte)itemNode.Type);
            itemType.SetType((byte)itemNode.Type);

            itemType.ParseOTFlags(itemNode.Flags);

            return itemType;

        }
    }

}