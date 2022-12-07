using System;
using System.Collections.Generic;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Factories;

namespace NeoServer.Extensions.Quests;

public class Quest : BaseItem, IUsable
{
    public Quest(IItemType metadata, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        : base(metadata, location)
    {
    }

    public void Use(IPlayer usedBy)
    {
        Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.ActionId, out var actionId);

        var item = ItemFactory.Instance.Create(actionId, usedBy.Location,
            new Dictionary<ItemAttribute, IConvertible> { { ItemAttribute.Count, 1 } });

        if (item is null || !item.IsPickupable ) return;

        if (usedBy.Inventory.BackpackSlot is not { } container 
            || !container.AddItem(item, true).Succeeded)
        {
            OperationFailService.Send(usedBy, $"You have found {item.FullName}, but you have no room to take it.");
            return;
        }

        NotificationSenderService.Send(usedBy, $"You have found {item.FullName}.", NotificationType.Information);
    }
}