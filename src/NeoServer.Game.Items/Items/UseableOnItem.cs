using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Useables;

namespace NeoServer.Game.Items.Items
{
    public abstract class UseableOnItem : MoveableItem, IPickupable, IUseableOnItem
    {
        public UseableOnItem(IItemType type, Location location) : base(type, location)
        {
        }

        public static bool IsApplicable(IItemType type) => type.Flags.Contains(Common.ItemFlag.Useable) && type.Flags.Contains(Common.ItemFlag.Pickupable);
        public abstract bool Use(ICreature usedBy, IItem onItem);
    }
}