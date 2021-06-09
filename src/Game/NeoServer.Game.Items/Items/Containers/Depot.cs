using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Containers;

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