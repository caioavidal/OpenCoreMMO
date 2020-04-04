using System.Collections.Generic;
using System.Collections.Immutable;
using NeoServer.Game.Contracts.Item;
using NeoServer.Game.Enums;
using NeoServer.OTB.Structure;
using NeoServer.Server.Model.Items;

public class ItemNodeParser
{
    private static IDictionary<ushort, IItemType> itemTypes;

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
        itemType.SetAlwaysOnTopOrder(itemNode.AlwaysOnTop);

        itemType.SetGroup((byte)itemNode.Type);
        itemType.SetType((byte)itemNode.Type);

        itemType.ParseOTFlags(itemNode.Flags);

        return itemType;

    }
}