using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Useables;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public abstract class UseableOnItem : MoveableItem, IPickupable, IUseableOnItem
    {
        protected UseableOnItem(IItemType type, Location location) : base(type, location)
        {
        }

        public abstract bool Use(ICreature usedBy, IItem onItem);

        public static bool IsApplicable(IItemType type)
        {
            return type.Flags.Contains(ItemFlag.Useable) && type.Flags.Contains(ItemFlag.Pickupable);
        }
    }
}