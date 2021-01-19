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
        public static new bool IsApplicable(IItemType type) =>type.Attributes.GetAttribute(Common.ItemAttribute.Type) == "depot";
    }
}
