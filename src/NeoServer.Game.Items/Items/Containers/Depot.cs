using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Containers;
using NeoServer.Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Items.Items.Containers
{
    public class Depot : Container, IDepot
    {
        public Depot(IItemType type, Location location) : base(type, location)
        {
            if(ItemTypeData.InMemory.TryGetValue(2594, out var itemType) is false) return;
            if (itemType is null) return;

            TryAddItem(new Container(itemType, location));
        }

        public static new bool IsApplicable(IItemType type) =>type.Attributes.GetAttribute(Common.ItemAttribute.Type) == "depot";
    }
}
