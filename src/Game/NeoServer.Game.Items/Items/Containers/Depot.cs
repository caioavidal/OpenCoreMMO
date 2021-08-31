using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Containers
{
    public class Depot : Container, IDepot
    {
        public Depot(IItemType type, Location location) : base(type, location)
        {
        }

        public new static bool IsApplicable(IItemType type)
        {
            return type.Attributes.GetAttribute(ItemAttribute.Type) == "depot";
        }
    }
}